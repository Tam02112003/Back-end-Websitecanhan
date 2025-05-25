using Amazon.DynamoDBv2.DataModel;

namespace Websitecanhan.Models
{
    [DynamoDBTable("Projects")]
    public class Project
    {
        [DynamoDBHashKey] // Partition key
        public int Id { get; set; }

        [DynamoDBProperty]
        public string Name { get; set; }

        [DynamoDBProperty]
        public string Description { get; set; }

        [DynamoDBProperty]
        public DateTime? StartDate { get; set; }

        [DynamoDBProperty]
        public DateTime? EndDate { get; set; }

        [DynamoDBProperty]
        public string Technologies { get; set; }

        [DynamoDBProperty]
        public string ImageUrl { get; set; }  // Lưu URL ảnh từ Cloudinary

        [DynamoDBProperty]
        public string ImagePublicId { get; set; }

        [DynamoDBProperty]
        public string GitHubLink { get; set; }
    }
}