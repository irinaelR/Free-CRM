using Domain.Entities;
using Infrastructure.DataAccessManager.EFCore.Contexts;
using Infrastructure.ExportManager;
using Microsoft.AspNetCore.Mvc;

namespace ASPNET.BackEnd.Controllers;

[Route("api/[controller]")]
public class ObjectCopyController : Controller
{
    private readonly IWebHostEnvironment _host;
    private DataContext _context;

    public ObjectCopyController(IWebHostEnvironment host, DataContext context)
    {
        _host = host;
        _context = context;
    }
    
    [HttpPost("Duplicate")]
    public ActionResult<string> DuplicateCampaign([FromBody] string campaignId)
    {
        try
        {
            (Campaign campaign, List<Budget> budgets, List<Expense> expenses) = EntityWriter.Find(campaignId, _context);
            EntityWriter writer = new EntityWriter(campaign, budgets, expenses);
            string fileName = "export.txt";
            string result = writer.GetFileContent();
            return Ok(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(e.Message);
        }
        
    }

    [HttpPost("SaveCampaign")]
    public string SaveCampaign([FromBody] WrittenCampaignRequest campaignRequest)
    {
        try
        {
            Campaign c = campaignRequest.MakeCampaign();
            _context.Campaign.Add(c);
            _context.SaveChanges();
            return c.Id;
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }
    
    [HttpPost("SaveBudget")]
    public string SaveCampaign([FromBody] WrittenBudgetRequest budgetRequest)
    {
        try
        {
            Budget c = budgetRequest.MakeBudget();
            _context.Budget.Add(c);
            _context.SaveChanges();
            return c.Id;
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }
    
    [HttpPost("SaveExpense")]
    public string SaveExpense([FromBody] WrittenExpenseRequest expenseRequest)
    {
        try
        {
            Expense c = expenseRequest.MakeExpense();
            _context.Expense.Add(c);
            _context.SaveChanges();
            return c.Id;
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }
}