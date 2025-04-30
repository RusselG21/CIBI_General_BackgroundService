namespace Talkpush_BackgroundService.Models;

//Deserialize JSON payload from Talkpush API

//Create candidate root
public class CandidateRoot
{
    public IEnumerable<Candidate> Candidates { get; set; } = [];
}

public class Candidate
{
    public int Id { get; set; }
    public int Candidate_id { get; set; } 
    public string First_Name { get; set; } = string.Empty;
    public string Last_Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;   
    public string User_Phone_Number { get; set; } = string.Empty;
    public Others Others { get; set; } = new();
}

public class Others
{
    public string? Middle_Name { get; set; }
    public string? Date_Of_Birth { get; set; }
    public string? Sss_Number { get; set; }
    public string? Tin_Number { get; set; }
    public string? Msa { get; set; }
    public string? Bi_Peme_Poc { get; set; }
    public string? Job_Requisition_Primary_Location { get; set; }
    public string? Job_Requisition_Id { get; set; }
}


public class CreateTicketCandidateRecord
{
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string? DateOfBirth { get; set; }
    public string? EmailAddress { get; set; }
    public string? PhoneNumber { get; set; }
    public string? SSSIDNumber { get; set; }
    public string? TIN { get; set; }
    public string? Remarks { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? RequestorFirstName { get; set; }
    public string? RequestorLastName { get; set; }
    public string? RequestorEmailAddress { get; set; }
    public string? Site { get; set; }
    public int TurnAroundTimeID { get; set; }
    public int ReportTypeID { get; set; }
    public int CountryID { get; set; }
    public int ProvinceID { get; set; }
    public int CityID { get; set; }
    public string? Address { get; set; }
    public string? PostalCode { get; set; }
}
