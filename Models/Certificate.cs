using Amazon.DynamoDBv2.DataModel;

namespace Websitecanhan.Models
{
    [DynamoDBTable("Certificates")]
    public class Certificate
    {
        [DynamoDBHashKey] // Partition key
        public int Id { get; set; }

        [DynamoDBProperty]
        public string Name { get; set; }

        [DynamoDBProperty]
        public DateTime? Year { get; set; }
    }
}