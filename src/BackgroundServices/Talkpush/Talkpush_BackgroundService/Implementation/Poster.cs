using Microsoft.EntityFrameworkCore.Diagnostics;
using Talkpush_BackgroundService.Data.DataAbstraction;

namespace Talkpush_BackgroundService.Implementation;

public class Poster(
     HttpClient httpClient, 
     ILogger<Poster> logger,
     ICreatedTicket_InsertData createdTicket_InsertData,
     ICheckedCandidateId checkedCandidateId) : IPoster
{
    public async Task<int> PostAsync(
        string url, 
        object payload, 
        object talkPushPayload,
        string bearerToken, 
        CancellationToken cancellationToken)
    {
        var payloadResponse = (Candidate)talkPushPayload;

        // Log for check if candidate id is already in the database 
        logger.LogInformation("Checking if candidate with ID {CandidateId} has already been processed and assigned a ticket.", payloadResponse.Id);
        var isCandidateIdAlreadyProcessed = await checkedCandidateId.CheckCandidateIdAsync(payloadResponse.Id, cancellationToken);

        if (isCandidateIdAlreadyProcessed)
        {
            logger.LogWarning("Candidate with ID {CandidateId} has already been processed and assigned a ticket.", payloadResponse.Id);
            return 200;
        }

        // Log for telling candidate id has not process yet or has no ticket
        logger.LogInformation("Candidate with ID {CandidateId} has not been processed yet or does not have an associated ticket.", payloadResponse.Id);


        logger.LogInformation("Posting payload to {Url}", url);

        // Validate parameters
        if (payload == null || string.IsNullOrEmpty(url) || string.IsNullOrEmpty(bearerToken))
        {
            logger.LogWarning("Invalid parameters: url: {Url}, payload: {Payload}, bearerToken: {BearerToken}", url, JsonSerializer.Serialize(payload), bearerToken);
            return 500;
        }

        // Create the request message
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(payload)
        };

        // Set the authorization header
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

        // Send the request
        var response = await httpClient.SendAsync(request, cancellationToken);

        var responseBody = await response.Content.ReadAsAsync<CreatedTicketResponse>();

        // logger for response body
        logger.LogWarning("Response from {Url}, Payload Request {PayloadRequest} , Response Body {ResponseBody}", 
            url, 
            JsonSerializer.Serialize(payload),
            JsonSerializer.Serialize(responseBody));

        // Log the response if not successful
        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning("Failed to POST payload. Status: {StatusCode}", response.StatusCode);
            return 400;
        }

        // Log the successful response
        logger.LogInformation("Payload successfully posted to {Endpoint}", url);

        // Log for inserting created ticket data
        logger.LogInformation("Inserting created ticket data to database OMS");
        var insertCreatedTicketData = new CreatedTicket
        {
            Id = Guid.NewGuid(),
            Candidate_Primary_Id = payloadResponse!.Id,
            Candidate_Id = payloadResponse.Candidate_id,
            TicketCreated = DateTime.UtcNow,
            TicketNumber = responseBody!.TicketNumber
        };

        await createdTicket_InsertData.InsertDataAsync(insertCreatedTicketData, cancellationToken);

        // log Successfully inserted 
        logger.LogInformation("Successfully inserted created ticket data to database OMS");

        return 200;

    }
}
