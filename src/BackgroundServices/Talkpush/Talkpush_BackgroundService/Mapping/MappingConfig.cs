namespace Talkpush_BackgroundService.Mapping;

public class MappingConfig
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig<Candidate, CreateTicketCandidateRecord>.NewConfig()
            .Map(dest => dest.FirstName, src => src.First_Name)
            .Map(dest => dest.MiddleName, src => src.Others.Middle_Name)
            .Map(dest => dest.LastName, src => src.Last_Name)
            .Map(dest => dest.DateOfBirth, src => src.Others.Date_Of_Birth)
            .Map(dest => dest.EmailAddress, src => src.Email)
            .Map(dest => dest.PhoneNumber, src => src.User_Phone_Number)
            .Map(dest => dest.SSSIDNumber, src => src.Others.Sss_Number ?? "")
            .Map(dest => dest.TIN, src => src.Others.Tin_Number ?? "")
            .Map(dest => dest.Remarks, src => src.Others.Msa ?? "")
            .Map(dest => dest.ReferenceNumber, src => "")
            .Map(dest => dest.RequestorFirstName, src => "Andrea Ross")
            .Map(dest => dest.RequestorLastName, src => "Cueto")
            .Map(dest => dest.RequestorEmailAddress, src => "noemail@noemail.com")
            .Map(dest => dest.Site, src => "CIBI_TEST") //src.Others.Job_Requisition_Primary_Location ?? ""
            .Map(dest => dest.TurnAroundTimeID, src => 1)
            .Map(dest => dest.ReportTypeID, src => 424)
            .Map(dest => dest.CountryID, src => 0)
            .Map(dest => dest.ProvinceID, src => 0)
            .Map(dest => dest.CityID, src => 0)
            .Map(dest => dest.Address, src => "")
            .Map(dest => dest.PostalCode, src => "")
            .AfterMapping((src, dest) =>
            {
                // Perform any additional transformations here
                dest.EmailAddress = dest.EmailAddress!.ToLowerInvariant();

                //contact Number adding 0
                dest.PhoneNumber = dest.PhoneNumber!.StartsWith("0") ? dest.PhoneNumber : "0" + dest.PhoneNumber;

                // Convert the date of birth to the desired format
                dest.DateOfBirth = DateTime.Parse(src.Others.Date_Of_Birth!).ToString("yyyy/MM/dd");
            });
    }
}
