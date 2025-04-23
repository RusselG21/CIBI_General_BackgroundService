namespace Talkpush_BackgroundService.Models;

//Deserialize JSON payload from Talkpush API

//Create candidate root
public class CandidateRoot
{
    public IEnumerable<Candidate> Candidates { get; set; } = [];
}

public class Candidate
{
    public string First_Name { get; set; } = string.Empty;
    public string Last_Name { get; set; } = string.Empty;
    public long Candidate_Id { get; set; } 
    public string Email { get; set; } = string.Empty;
    public string User_Phone_Number { get; set; } = string.Empty;
    public Others Others { get; set; } = new();
}

public class Others
{
    public string? Middle_Name { get; set; }
    public DateTime Date_Of_Birth { get; set; }
    public string? Sss_Number { get; set; }
    public string? Tin_Number { get; set; }
    public string? Msa { get; set; }
    public string? Bi_Peme_Poc { get; set; }
    public string? Job_Requisition_Primary_Location { get; set; }
    public string? Job_Requisition_Id { get; set; }
}


public record CreateTicketCandidateRecord(
    string FirstName,
    string? MiddleName,
    string LastName,
    DateTime DateOfBirth,
    string EmailAddress,
    string PhoneNumber,
    string SSSIDNumber,
    string TIN,
    string Remarks,
    string ReferenceNumber,
    string RequestorFirstName,
    string RequestorLastName,
    string RequestorEmailAddress,
    string Site,
    int TurnAroundTimeID,
    int ReportTypeID,
    int CountryID,
    int ProvinceID,
    int CityID,
    string Address,
    string PostalCode
);
