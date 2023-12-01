namespace Trivesta.Model
{
    public class ResponseMessage<T>
    {
        public string Message { get; set; }
        public int StatusCode { get; set; } = 201;
        public T Data { get; set; }
        public int Flag { get; set; }
        public string Action { get; set; }
        public string Control { get; set; }
        public string Tag { get; set; }
    }
}