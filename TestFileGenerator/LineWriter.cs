namespace TestFileGenerator;

//based on: https://www.dotnetperls.com/random-paragraphs-sentences
public static class LineWriter
{
    static readonly string[] _words = ["apple", "banana", "is", "yellow", "Cherry", "best", "something"];

    private static string RandomWord => _words[Random.Shared.Next(_words.Length)];

    public static long WriteLine(TextWriter writer)
    {
        const char dot = '.';
        const char space = ' ';

        int wordsCount = Random.Shared.Next(1, 10);


        // initial size = number + dot + wordsCount*space
        long sizeInBytes = (wordsCount + 1) + writer.NewLine.AsSpan().Length;

        var number = Random.Shared.Next();
        sizeInBytes += (int)Math.Floor(Math.Log10(number)) + 1;
        writer.Write(number);
        writer.Write(dot);
        writer.Write(space);

        var word = RandomWord.AsSpan();

        sizeInBytes += word.Length;

        writer.Write(char.ToUpper(word[0]));
        writer.Write(word[1..]);
        writer.Write(space);

        for (int i = 1; i < wordsCount - 1; i++)
        {
            word = RandomWord.AsSpan();
            writer.Write(word);
            sizeInBytes += word.Length;

            writer.Write(space);
        }

        word = RandomWord.AsSpan();
        writer.Write(word);
        sizeInBytes += word.Length;

        writer.WriteLine();

        return sizeInBytes;
    }


}
