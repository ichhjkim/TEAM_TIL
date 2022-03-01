# ExceptionHandler 예외 처리


## HandlerExceptionResolver 적용 전

발생하는 예외에 따라 400, 403, 404등 다른 상태코드로도 처리해야할 때가 있다.
오류메시지, 형식등을 다르게 처리해야한다.


### 상태코드 변환
예를 들어 `IllegalArgumentException`을 처리하지 못하는 경우, HTTP 상태코드를 400으로 처리하고 싶다. 어떻게 하면 좋을까?

`ApiExceptionController.class` 수정

```java
@GetMapping("/api/members/{id}")
public MemberDto getMember(@Pathvariable("id") String id) {
    if (id.equals("ex")) {
        throw new RuntimeException();
    }
    else if (id.equals("bad")) {
        throw new IllegalArgumentException();
    }
    return new MemberDto(id);
}
```

=> 실행해보면 상태코드가 500인 것을 확인할 수 있다.

## HandlerExceptionResolver
스프링 MVC는 컨트롤러(핸들러) 밖으로 예외가 던져진 경우, 예외를 해결하고 동작을 재정의할 수 있는 방법을 제공하고 있다.
이때 HandlerExceptionResolver를 사용할 수 있으며, 보통 ExceptionResolver라고 부른다.

참고로 ExceptionResolver를 호출해도 postHandle()은 호출되지 않는다.

### HandlerExceptionResolver Implement

```java
package co.kr.sample.resolver;

import org.springframework.web.servlet.HandlerExceptionResolver;

@Slf4j
public class MyHandlerExceptionResolver implements HandlerExceptionResolver {
    @Override
    public ModelAndView resolverException(HttpServletRequest request, HttpServletResponse response, Object handler) {
        try {
            if (ex instanceof IllegalArgumentException {
                response.sendError(HttpServletResponse.SC_BAD_REQUEST, ex.getMessage());
                return new ModelAndView();
            }
        } catch (IOException e) {
            log.info("error : {}", e);
        }
        return null;
    }
}
```

MyHandlerExceptionResolver를 WebConfig에 등록

```java
@Configuration 
public class WebConfig implements WebMvcConfiguerer {

    @Override
    public void extendHandlerExceptionHandler(List<HandlerExceptionResolver> resolvers) {
        resolvers.add(new MyHandlerExceptionResolver());
    }
}
```
