using MongoDB.Bson;
using MongoDB.Driver;


var client = new MongoClient();

// 1. 데이터 베이스 가져오기 
var database = client.GetDatabase("foo");

// 2. 컬렉션 삭제하기 
database.DropCollection("bar");

// 3. 컬렉션 가져오기 (없으면 생성)
var collection = database.GetCollection<BsonDocument>("bar");



// 1. Document 삽입하기
// {
//      "name": "MongoDB",
//      "type": "database",
//      "count": 1,
//      "info": {
//          x: 203,
//          y: 102
//      }
// }

var document = new BsonDocument
{
    { "name", "MongoDB" },
    { "type", "Database" },
    { "count", 1 },
    { "info", new BsonDocument
        {
            { "x", 203 },
            { "y", 102 }
        }}
};

// 동기 입력
collection.InsertOne(document);
// 비동기 입력
//await collection.InsertOneAsync(document);

Console.WriteLine(database);
//Console.WriteLine("abc");

// 2. 한 번에 여러 개의 document 입력하기 
var documents = Enumerable.Range(0, 10).Select(i => new BsonDocument("counter", i));
collection.InsertMany(documents);
var documents2 = Enumerable.Range(5, 10).Select(i => new BsonDocument("counter", i));
collection.InsertMany(documents2);
//await collection.InsertManyAsync(documents);

// 3. 갯수 세기
var count = collection.CountDocuments(new BsonDocument());
Console.WriteLine($"Document 개수 : {count}");

// 4. 컬렉션에서 데이터 찾기 

var find_document = collection.Find(new BsonDocument()).FirstOrDefault();
Console.WriteLine(find_document);

// 5. 컬렉션 전체 데이터 반환 받기

var find_documents = collection.Find(new BsonDocument()).ToList();
Console.WriteLine(find_documents);

// 하나씩 출력하기
// await collection.Find(new BsonDocument()).ForEachAsync(d => Console.WriteLine(d));

// // 6. 커서 사용하기
// var cursor = collection.Find(new BsonDocument()).ToCursor();
// foreach(var doc in cursor.ToEnumerable())
// {
//     Console.WriteLine(doc);    
// }

//7. 필터로 조회하기
// 없는 데이터로 조회하면 Exception 난다. 
var filter  = Builders<BsonDocument>.Filter.Eq("counter",5);
//var filter_document = collection.Find(filter).First();
var filter_document = collection.Find(filter).FirstOrDefault();
if (filter_document != null)
    Console.WriteLine(filter_document);
else
    Console.WriteLine("null");

//8. 필터로 여러개 조회하기
Console.WriteLine("filter Multi");
filter  = Builders<BsonDocument>.Filter.Gt("counter",7);


//var filter_document = collection.Find(filter).First();
var cursor = collection.Find(filter).ToList();
foreach(var doc in cursor)
{

    Console.WriteLine(doc);
}

// 9. 다양한 필터 조합하기 기본
Console.WriteLine("filter Multi ++");
var filterBuilder = Builders<BsonDocument>.Filter;
filter = filterBuilder.Gt("counter", 7) & filterBuilder.Lt("counter", 10);

//var filter_document = collection.Find(filter).First();
cursor = collection.Find(filter).ToList();
foreach(var doc in cursor)
{

    Console.WriteLine(doc);
}

// 10. 정렬하기
Console.WriteLine("Sort Multi ");
//var sort = Builders<BsonDocument>.Sort.Descending("counter");
var sort = Builders<BsonDocument>.Sort.Ascending("counter");
var sorted_documents = collection.Find(filter).Sort(sort).ToList();
foreach(var doc in sorted_documents)
{
    Console.WriteLine(doc);
}

// 11. 값 제외하기 (Projecting Fields)
Console.WriteLine("Projecting Fields ");
var projection = Builders<BsonDocument>.Projection.Exclude("_id");

var projected_documents = collection.Find(filter).Project(projection).ToList();
foreach(var doc in projected_documents)
{
    Console.WriteLine(doc);
}

// 12. 값 수정하기 
Console.WriteLine("Updating Data ");
var update_filter = Builders<BsonDocument>.Filter.Eq("counter", 1);
var update = Builders<BsonDocument>.Update.Set("counter", 100);

var result = collection.UpdateOne(update_filter, update);

Console.WriteLine(result.ModifiedCount);

// 13. 값 삭제하기
Console.WriteLine("Updating Data ");
var delete_filter = Builders<BsonDocument>.Filter.Eq("counter", 100);

var delete_result = collection.DeleteOne(delete_filter);

Console.WriteLine(delete_result.DeletedCount);