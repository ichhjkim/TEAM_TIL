이 챕터에서는 EventPublishingRepositoryProxyPostProcessor 클래스와 AbstractAggregateRoot 기반 registerEvent 동작원리에 대해 적어보았습니다.

## registerEvent()

Spring data의 save동작 시, AbstractAggregateRoot를 상속하는 도메인 모델에서 event를 등록하는 방법에 대해 기록하려합니다.  
특정 entity가 저장 또는 삭제 될 때, event호출을 통하여 특정 동작을 수행하게 할 수 있습니다.

이해의 편의성을 돕기위해, 고객 도메인의 계정 애그리거트로 예시를 들어보겠습니다.

### 계정(Account) 생성 이벤트가 발생했을 때

#### 1) 계정 애그리거트 이벤트 발행

```
@Getter
@Entity
@Table(name = "account_info")
@NoArgsConstructor(access = AccessLevel.PROTECTED)
public class Account extends AbstractAggregateRoot<Account> implements Serializable {

    public Account(String loginId, CustomerId customerId,     String newPassword, String channel) throws     NoSuchAlgorithmException {
    this.customerId = customerId;
    this.loginId = new LoginId(loginId);
    this.password = new Password(newPassword, channel);
    registerEvent(new PasswordChangedEvent(
            this.customerId,
            this.password
    ));
}
}
```

#### 2) 비밀번호 변경 이벤트

```
@Getter
public class PasswordChangedEvent {
    private CustomerId customerId;
    private Password password;

    public PasswordChangedEvent(CustomerId customerId, Password password) {
        this.customerId = customerId;
        this.password = password;
    }
}
```

#### 계정 생성 이벤트 핸들러

```
@RequiredArgsConstructor
public class AccountHistoryEventHandler {

    private final CreateAccountHistoryService createAccountHistoryService;

    @EventListener
    public void add(PasswordChangedEvent passwordChangedEvent) {
        createAccountHistoryService.createAccountHistory(passwordChangedEvent.getCustomerId(), passwordChangedEvent.getPassword());
    }
}
```

#### spring data의 save() Code

```
// 개인정보 변경(비밀번호변경)
@Transactional
public void updatePassword(String customerId, UpdatePasswordCommand updatePasswordCommand) throws Exception {
    Customer customer = customerQueryService.findByCustomerId(customerId);
    Account account = accountQueryService.findByCustomerId(customer.getCustomerId().getCustomerId());
    account.updatePassword(updatePasswordCommand, customer, account);
    accountRepository.save(account);

    syncCustomerService.updatePassword(new SyncUpdatePasswordReqDTO(account.getPassword().getPassword()), account.getCustomerId().getCustomerId());
}
```

save과정에서 코드를 따라가다 보면 다음 EventPublishingRepositoryProxyPostProcessor 클래스에서 해당 함수가 호출되게 됩니다.

EventPublishingRepositoryProxyPostProcessor.class의 주석 일부

RepositoryProxyPostProcessor to register a MethodInterceptor to intercept CrudRepository.save(Object) and CrudRepository.delete(Object) methods and publish events potentially exposed via a method annotated with _DomainEvents_.

RepositoryProxyPostProcessor는 CRUDRepository의 save, delete 메서드를 인터셉트하여 MethodInterceptor을 등록하고, @DomainEvents로 어노테이티드된 메서드의 이벤트를 발행합니다.

작성자:Oliver Gierke, Christoph Strobl, Yuki Yoshida, Réda Housni Alaoui

---

EventPublishingRepositoryProxyPostProcessor.class의 publishEventFrom(aggregateRoot의 registerEvent()에서 등록한 이벤트가 발행되는 곳입니다)

```
public void publishEventsFrom(@Nullable Object object, ApplicationEventPublisher publisher) {

  if (object == null) {
    return;
  }

  for (Object aggregateRoot : asCollection(object)) {

    for (Object event : asCollection(ReflectionUtils.invokeMethod(publishingMethod, aggregateRoot))) {
      publisher.publishEvent(event);
    }

    if (clearingMethod != null) {
      ReflectionUtils.invokeMethod(clearingMethod, aggregateRoot);
    }
  }
}
```

**참고**  
DomainEvent는 AggregateRoot에 annotated되어 있습니다.

```
package org.springframework.data.domain;

... 생략

public class AbstractAggregateRoot<A extends AbstractAggregateRoot<A>> {

    private transient final @Transient List<Object> domainEvents = new ArrayList<>();

    protected <T> T registerEvent(T event) {

        Assert./notNull/(event, "Domain event must not be null!");

        this.domainEvents.add(event);
        return event;
    }

    /**
     * Clears all domain events currently held. Usually invoked by the infrastructure in place in Spring Data/
     * repositories.
     */
    @AfterDomainEventPublication
    protected void clearDomainEvents() {
        this.domainEvents.clear();
    }

    /**
      * All domain events currently captured by the aggregate./
      */
    @DomainEvents
    protected Collection<Object> domainEvents() {
        return Collections./unmodifiableList/(domainEvents);
    }
     ... 생략
 }
```

이 밖에 관련 스프링 이벤트, CRUD 관련 소스는 spring-data 내 클래스 참고 부탁드립니다.  
이벤트 관련 소스는 org.springframework.data.domain에서 확인 가능합니다.
