namespace TalkPush.API.Operations;

public record UpdateReportDCommand(int Id, IFormFile? file, string? document_tag_name, string? document_tag_id) : IRequest<UpdateReportResult> ;

public record UpdateReportResult(bool IsSuccess);

public class UpdateReportDCommandValidator : AbstractValidator<UpdateReportDCommand>
{
    public UpdateReportDCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
        RuleFor(x => x.file).NotEmpty().WithMessage("File is required.");
        RuleFor(x => x.document_tag_name).NotEmpty().WithMessage("Document tag name is required.");
        RuleFor(x => x.document_tag_id).NotEmpty().WithMessage("Document tag id is required.");
    }
}


public class UpdateReportHandler(
    IConfiguration configuration,
    HttpClient httpClient) : IRequestHandler<UpdateReportDCommand, UpdateReportResult>
{
    public async Task<UpdateReportResult> Handle(UpdateReportDCommand request, CancellationToken cancellationToken)
    {
        var baseUrl = configuration["TalkpushSettings:BaseUrl"];
        var apiKey = configuration["TalkpushSettings:APIKey"];

        // Set up form-data content type    
        using var formData = new MultipartFormDataContent();

        // Add the file to the form-data
        using var stream = request!.file!.OpenReadStream();
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(request.file.ContentType ?? "application/octet-stream");
        formData.Add(fileContent, "file", request.file.FileName);

        // Add document_tag_name and document_tag_id
        formData.Add(new StringContent(request.document_tag_name ?? ""), "document_tag_name");
        formData.Add(new StringContent(request.document_tag_id ?? ""), "document_tag_id");

        // Construct the URL
        var url = baseUrl!.Replace("{id}",request.Id.ToString());
        var finalUrl = $"{url}?api_key={apiKey}";

        // Make the PUT request
        var response = await httpClient.PutAsync(finalUrl, formData, cancellationToken);

        // Check the response status
        if (!response.IsSuccessStatusCode)
        {
            return new UpdateReportResult(false);
        }

        return new UpdateReportResult(true);
    }
}
