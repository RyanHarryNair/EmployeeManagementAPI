namespace EmployeeManagementAPI.DTOs
{
    public class ResponseDto<T> // This line defines a generic class named ResponseDto with a type parameter T. The T represents the type of objects contained in the Result property.
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public List<T> Result { get; set; }
        /*This property represents the result of the response. 
         * It is a generic list (List<T>) that can hold objects of type T. 
         * The Result property is used to store the list of objects returned as a result of an operation.*/
    }
}
