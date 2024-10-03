namespace ASGBackend.Models
{
    public class GeminiResponse
    {
        public candidate[] candidates { get; set; }

        public GeminiResponse(string text)
        {
            candidates = new[] { new candidate { content = new content { parts = new[] { new part { text = text } } } } };
        }
    }

    public class GeminiRequest
    {
        public candidate[] candidates { get; set; }

        public GeminiRequest(string text)
        {
            candidates = new[] { new candidate { content = new content { parts = new[] { new part { text = text } } } } };
        }
    }

    public class candidate
    {
        public content content { get; set; }
    }

    public class content
    {
        public part[] parts { get; set; }
    }

    public class part
    {
        public string text { get; set; }
    }
}
