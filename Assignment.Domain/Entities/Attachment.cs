using System.ComponentModel.DataAnnotations;
using Assignment.Domain.Exceptions;

namespace Assignment.Domain.Entities;

public class Attachment
{
    public static Attachment Create(string name, string fileName, string link)
    {
        return new Attachment(name, fileName, link);
    }

    public static Attachment Hydrate(Guid id, string name, string fileName, string link)
    {
        return new Attachment(id, name, fileName, link);
    }

    private Attachment(string name, string fileName, string link)
    {
        if (!IsValidName(name, out var validationErrorMessageName))
        {
            throw new DomainException(validationErrorMessageName);
        }

        if (!IsValidFileName(fileName, out var validationErrorMessageFileName))
        {
            throw new DomainException(validationErrorMessageFileName);
        }

        if (!IsValidLink(link, out var validationErrorMessageLink))
        {
            throw new DomainException(validationErrorMessageLink);
        }

        Id = Guid.CreateVersion7();
        Name = name;
        FileName = fileName;
        Link = link;
    }

    private Attachment(Guid id, string name, string fileName, string link)
    {
        Id = id;
        Name = name;
        FileName = fileName;
        Link = link;
    }

    public Guid Id { get; }
    public string Name { get; private set; }
    public string FileName { get; }
    public string Link { get; }

    internal void Rename(string newName)
    {
        if (!IsValidName(newName, out var validationErrorMessage))
        {
            throw new DomainException(validationErrorMessage);
        }

        Name = newName;
    }

    private static bool IsValidName(string name, out string? validationErrorMessage)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            validationErrorMessage = "Name must not be empty.";
            return false;
        }

        validationErrorMessage = null;
        return true;
    }

    private static bool IsValidFileName(string fileName, out string? validationErrorMessage)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            validationErrorMessage = "File name must not be empty.";
            return false;
        }

        validationErrorMessage = null;
        return true;
    }

    private static bool IsValidLink(string link, out string? validationErrorMessage)
    {
        if (!Uri.TryCreate(link, new UriCreationOptions { }, out _))
        {
            validationErrorMessage = "Link must be a valid URI.";
            return false;
        }

        validationErrorMessage = null;
        return true;
    }

}