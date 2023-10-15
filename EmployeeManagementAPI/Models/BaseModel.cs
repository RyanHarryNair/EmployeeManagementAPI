namespace EmployeeManagementAPI.Models
{
    public class BaseModel // This Model is for Auditing Purpose; who & when it was created/update by & date. The properties of these model is to be inherited by other models because for eg when you add user, u need to see who added & by when the new user was added
    {
        // No Primary Key here becasue we need this model to store data only.
        public int CreatedBy { get; set; } // store the Id of the user who created
        public int UpdatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
//  We have the model ready, now we need someone to communicate with the database...ie Context