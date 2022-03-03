-- SQLite
-- 1. 데이터 정렬하기
SELECT prod_name FROM Products;

SELECT prod_name 
FROM Products 
ORDER BY prod_name;

-- 2. 여러개의 열로 정렬하기
-- ORDER BY 의 순서대로 정렬된다. prod_price 우선 정렬하면서 prod_price가 같을 때만 prod_name으로 정렬됨
SELECT prod_id, prod_price, prod_name
FROM Products
ORDER BY prod_price, prod_name;

-- SELECT prod_id, prod_price, prod_name
-- FROM Products
-- ORDER BY prod_name, prod_price;

-- 3. 열 위치로 정렬하기 
-- SELECT 문에 사용하는 컬럼 열의 index로 조회가능
-- index는 1부터 시작한다. 다음 예제는 1~4까지 허용가능

-- 열 이름을 안적어서 잘못된 열을 지정할 가능성이 있다. 
-- SELECT 문 수정시에 ORDER BY 수정을 안할 수도 있다. 
-- SELECT 문에 없는 값으로는 정렬이 불가능하다.
SELECT prod_id, prod_price, prod_name, vend_id
FROM Products
ORDER BY 1,3, prod_desc;

-- 혼용도 가능하다.
SELECT prod_id, prod_name, vend_id
FROM Products
ORDER BY 1, prod_price;

-- 내림차순은 DESC 추가
SELECT prod_id, prod_price, prod_name
FROM Products
ORDER BY prod_price DESC;

-- DESC 는 지정된 열에만 적용된다. 따라서 prod_price는 내림차순, 같은 값이면 prod_name은 오름차순으로 정렬됨
SELECT prod_id, prod_price, prod_name
FROM Products
ORDER BY prod_price DESC, prod_name;

-- <도전과제>
-- 1.
SELECT cust_name 
FROM Customers
ORDER BY cust_name DESC;

-- 2.
SELECT cust_id, order_num
FROM Orders
ORDER BY  cust_id, order_date DESC;

-- 3.
SELECT prod_id, quantity, item_price
FROM OrderItems
ORDER BY 2 DESC, 3 DESC;

-- 4. 잘못된거 찾기
-- SELECT vend_name,
-- FROM Vendors
-- ORDER vend_name DESC;

-- BY 빠짐, SELECT 뒤에 , 더 있음