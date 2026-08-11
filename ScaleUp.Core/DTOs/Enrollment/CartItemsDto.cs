namespace ScaleUp.Core.DTOs;

public class CartItemsDto
{
    //Enrollment Id
    public int Id { get; set; }
    public int CourseId { get; set; }
    public CourseDto Course { get; set; }
}
