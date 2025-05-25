using Microsoft.IO;

namespace WebAPI.Middlewares;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<RequestLoggingMiddleware> _logger = logger;
    private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager = new();

    public async Task Invoke(HttpContext context)
    {
        await LogRequest(context);

        await LogResponse(context);
    }

    private async Task LogRequest(HttpContext context)
    {
        context.Request.EnableBuffering();
        await using var requestStream = _recyclableMemoryStreamManager.GetStream();
        await context.Request.Body.CopyToAsync(requestStream);
        _logger.LogInformation($"Http Request Information:{Environment.NewLine}" +
                               $"Schema:{context.Request.Scheme} " +
                               $"Host: {context.Request.Host} " +
                               $"Path: {context.Request.Path} " +
                               $"QueryString: {context.Request.QueryString} " +
                               $"Request Body: {ReadStreamInChunks(requestStream)}");
        context.Request.Body.Position = 0;
    }

    private static string ReadStreamInChunks(Stream stream)
    {
        const int readChunkBufferLength = 4096;
        stream.Seek(0, SeekOrigin.Begin);
        using var textWriter = new StringWriter();
        using var reader = new StreamReader(stream);
        var readChunk = new char[readChunkBufferLength];
        int readChunkLength;
        do
        {
            readChunkLength = reader.ReadBlock(readChunk,
                                               0,
                                               readChunkBufferLength);
            textWriter.Write(readChunk, 0, readChunkLength);
        } while (readChunkLength > 0);
        return textWriter.ToString();
    }

    private async Task LogResponse(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;

        using var responseBody = _recyclableMemoryStreamManager.GetStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            _logger.LogInformation($"Http Response Information:{Environment.NewLine}" +
                           $"Schema:{context.Request.Scheme} " +
                           $"Host: {context.Request.Host} " +
                           $"Path: {context.Request.Path} " +
                           $"QueryString: {context.Request.QueryString} " +
                           $"Response Body: {text}");

            if (originalBodyStream.CanWrite)
            {
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }
}