[HttpPost]
public ActionResult AddEmployee(Employee emp)
{
    if (!ModelState.IsValid)
    {
        var errors = ModelState.Select(x => new { 
            Field = x.Key, 
            Error = x.Value.Errors.FirstOrDefault()?.ErrorMessage 
        }).Where(x => x.Error != null).ToList();
        
        return Json(new { 
            success = false, 
            errors = errors
        });
    }
    
    // Additional validation
    if (emp.dob >= DateTime.Today)
    {
        ModelState.AddModelError("dob", "Date of birth cannot be in the future");
        return Json(new { 
            success = false, 
            errors = new[] { new { Field = "dob", Error = "Date of birth cannot be in the future" } }
        });
    }
    
    try
    {
        // ... existing code ...
    }
    catch (Exception ex)
    {
        Response.StatusCode = 500;
        return Json(new { 
            success = false, 
            message = "Error adding employee",
            errors = new[] { new { Field = "", Error = "An error occurred while saving the employee." } }
        });
    }
}