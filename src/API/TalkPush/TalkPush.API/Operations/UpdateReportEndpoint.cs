namespace TalkPush.API.Operations;

public record UpdateReportRequest(int Id, IFormFile? file, string? document_tag_name, string? document_tag_id);

public record UpdateReportResponse(bool IsSuccess);

public class UpdateReportEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/report", async (HttpRequest request, ISender sender) =>
        {
            if (!request.HasFormContentType)
                return Results.BadRequest("Expected multipart/form-data");

            // Manually read form data
            var form = await request.ReadFormAsync();

            // Extract fields from form data
            var id = int.Parse(form["Id"]!);
            string documentTagName = form["document_tag_name"]!;
            string documentTagId = form["document_tag_id"]!;
            IFormFile file = form.Files.GetFile("file")!;



            // Map to command (if using AutoMapper or Mapster)
            var command = new UpdateReportDCommand(id, file, documentTagName, documentTagId);

            // Send the command via MediatR
            var result = await sender.Send(command);

            // Map the result to response
            var response = result.Adapt<UpdateReportResponse>();

            // Return the response
            return Results.Ok(response);
        })
          .WithName("UpdateReport")
          .Produces<UpdateReportResponse>(StatusCodes.Status200OK)
          .ProducesProblem(StatusCodes.Status400BadRequest)
          .ProducesProblem(StatusCodes.Status404NotFound)
          .WithSummary("Update Report")
          .WithDescription("Update Report");

    }
}

