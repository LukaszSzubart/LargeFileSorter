namespace Tools;

public static class StreamLineCounter
{
    private const char CR = '\r';
    private const char LF = '\n';
    private const char NULL = (char)0;

    public static long Count(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        long lineCount = 0L;

        byte[] byteBuffer = new byte[BinarySize.InBytes.MB];
        char detectedEOL = NULL;
        char currentChar = NULL;

        int bytesRead;
        while ((bytesRead = stream.Read(byteBuffer, 0, byteBuffer.Length)) > 0)
        {
            for (int i = 0; i < bytesRead; i++)
            {
                currentChar = (char)byteBuffer[i];

                if (detectedEOL != NULL)
                {
                    if (currentChar == detectedEOL)
                    {
                        lineCount++;
                    }
                }
                else if (currentChar == LF || currentChar == CR)
                {
                    detectedEOL = currentChar;
                    lineCount++;
                }
            }
        }

        // We had a NON-EOL character at the end without a new line
        if (currentChar != LF && currentChar != CR && currentChar != NULL)
        {
            lineCount++;
        }
        return lineCount;
    }
}
