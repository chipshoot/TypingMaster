﻿namespace TypingMaster.Business.Models;

public class Lesson
{
    public int Id { get; set; }

    // The target key or works to practice
    public IEnumerable<string> Target { get; set; } = [];

    public string Instruction { get; set; } = null!;

    public string PracticeText { get; set; } = null!;

    /// <summary>
    /// The point that measure the difficulty of the lesson
    /// </summary>
    public int Point { get; set; }

    public string? Description { get; set; }
}