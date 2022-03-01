## API 예외처리

API는 각 오류 상황에 맞는 오류 응답 스펙을 정하고,  JSON으로 데이터를 내려주어야 한다.

```java
@Slf4j
@RestController
public class ApiExceptionController {
	@GetMapping(“/api/member/{id}”)
	public MemberDto getMember(@PathVariable(“id”) String id) {
	if (id.equals(“ex”)) {
		throw new RuntimeException(“잘못된 사용자”)
	}
	return new MemberDto(id, “hello”+id);
}
```

```java
@RequestMapping(value = "/error-page/500")
public String errorPage500(HttpServletRequest request, HttpServletResponse response) {
    return "/error-page/500";
}

// 클라이언트가 "Accept: application/json"으로 호출시, 이 메서드가 더 우선순위를 가지고 호출된다.
@RequestMapping(value = "/error-page/500", produces = MediaType.APPLICATION_JSON_VALUE) 
public ResponseEntity<Map<String, Object>> errorApi500(HttpServletRequest request, HttpServletResponse response) {
    log.info("error API");
    Map<String, Object> result = new HashMap<>();
    Exception ex = (Exception)request.getAttribute(ERROR_EXCEPTION);
    result.put("status", request.getAttribute(ERROR_STATUS_CODE));
    result.put("message", ex.getMessage());
    Integer statusCode = (Integer)request.getAttribute(RequestDispatcher.ERROR_STATUS_CODE);
    return new ResponseEntity<>(result, HttpStatus.valueOf(statusCode));
}
```

