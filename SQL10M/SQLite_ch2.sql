-- SQLite
-- 1. 기본 조회하기
SELECT prod_name
FROM Products;

-- 2. 여러 개의 열 조회하기
SELECT prod_id, prod_name, prod_price
FROM Products;

-- 3. 모든 열 조회하기
SELECT *
FROM Products;

-- 4. 중복출력 방지하기 
-- DISTINCT 키워드는 모든 열에 일괄 적용된다. 한 개의 열에만 부분적으로 적용불가
SELECT DISTINCT vend_id
FROM Products;

SELECT DISTINCT vend_id, prod_price
FROM Products;

SELECT vend_id, prod_price
FROM Products;

--5. 결과 제한하기
-- MSSQL
--SELECT TOP 5 prod_name From Products;
-- DB2 
--SELECT prod_name From Products FETCH FIRST 5 ROWS ONLY;
-- Oracle
--SELECT prod_name FROM Products WHERE ROWNUM <= 5;
-- Mysql, mariadb, postgresql, sqlite 
SELECT prod_name FROM Products LIMIT 5;

-- OFFSET 사용
-- 5개의 데이터를 띄우고 거기서 5개까지 조회, 데이터가 9개뿐이므로 실제 조회는 4개만 된다
SELECT prod_name FROM Products LIMIT 5 OFFSET 5;

-- <도전과제>
-- 1. 
SELECT cust_id FROM Customers;
-- 2. 
SELECT DISTINCT prod_id FROM OrderItems;
-- 3. 
SELECT * FROM Customers;
SELECT cust_id FROM Customers;
