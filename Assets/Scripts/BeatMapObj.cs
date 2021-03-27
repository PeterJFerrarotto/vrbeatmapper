using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[JsonObject]
public class DifficultyBeatmap
{
    [JsonProperty("_difficulty")]
    public string Difficulty { get; set; }
    [JsonProperty("_difficultyRank")]
    public int DifficultyRank { get; set; }
    [JsonProperty("_beatmapFileName")]
    public string BeatmapFilename { get; set; }
    [JsonProperty("_noteJumpMovementSpeed")]
    public int NoteJumpMovementSpeed { get; set; }
    [JsonProperty("_noteJumpStartBeatOffset")]
    public int NoteJumpStartBeatOffset { get; set; }
}

[JsonObject]
public class DifficultyBeatmapSet
{
    [JsonProperty("_beatmapCharacteristicName")]
    public string BeatmapCharacteristicName { get; set; }
    [JsonProperty("_difficultyBeatmaps")]
    public List<DifficultyBeatmap> DifficultyBeatmaps { get; set; }
}

[JsonObject]
public class SongInfo
{
    [JsonProperty("_version")]
    public string Version { get; set; }
    [JsonProperty("_songName")]
    public string SongName { get; set; }
    [JsonProperty("_songSubName")]
    public string SongSubName { get; set; }
    [JsonProperty("_songAuthorName")]
    public string SongAuthorName { get; set; }
    [JsonProperty("_levelAuthorName")]
    public string LevelAuthorName { get; set; }
    [JsonProperty("_beatsPerMinute")]
    public int BeatsPerMinute { get; set; }
    [JsonProperty("_shuffle")]
    public int Shuffle { get; set; }
    [JsonProperty("_shufflePeriod")]
    public double ShufflePeriod { get; set; }
    [JsonProperty("_previewStartTime")]
    public int PreviewStartTime { get; set; }
    [JsonProperty("_previewDuration")]
    public int PreviewDuration { get; set; }
    [JsonProperty("_songFilename")]
    public string SongFilename { get; set; }
    [JsonProperty("_coverImageFilename")]
    public string CoverImageFilename { get; set; }
    [JsonProperty("_environmentName")]
    public string EnvironmentName { get; set; }
    [JsonProperty("_songTimeOffset")]
    public int _songTimeOffset { get; set; }
    [JsonProperty("_difficultyBeatmapSets")]
    public List<DifficultyBeatmapSet> DifficultyBeatmapSets { get; set; }
}

[JsonObject]
public class Event
{
    [JsonProperty("_time")]
    public double Time { get; set; }
    [JsonProperty("_type")]
    public int Type { get; set; }
    [JsonProperty("_value")]
    public int Value { get; set; }
}

[JsonObject]
public class Note
{
    [JsonProperty("_time")]
    public double Time { get; set; }
    [JsonProperty("_lineIndex")]
    public int LineIndex { get; set; }
    [JsonProperty("_lineLayer")]
    public int LineLayer { get; set; }
    [JsonProperty("_type")]
    public int Type { get; set; }
    [JsonProperty("_cutDirection")]
    public int CutDirection { get; set; }
}

[JsonObject]
public class Obstacle
{
    [JsonProperty("_time")]
    public double Time { get; set; }
    [JsonProperty("_lineIndex")]
    public int LineIndex { get; set; }
    [JsonProperty("_type")]
    public int Type { get; set; }
    [JsonProperty("_duration")]
    public double Duration { get; set; }
    [JsonProperty("_width")]
    public int Width { get; set; }
}

[JsonObject]
public class SpecialEventsKeywordFilters
{
    [JsonProperty("_keywords")]
    public List<object> Keywords { get; set; }
}

[JsonObject]
public class SongDifficulty
{
    [JsonProperty("_version")]
    public string Version { get; set; }
    [JsonProperty("_events")]
    public List<Event> Events { get; set; }
    [JsonProperty("_notes")]
    public List<Note> Notes { get; set; }
    [JsonProperty("_obstacles")]
    public List<Obstacle> Obstacles { get; set; }
    [JsonProperty("_specialEventsKeywordFilters")]
    public SpecialEventsKeywordFilters SpecialEventsKeywordFilters { get; set; }
}

public enum NoteType
{
    LEFT = 0,
    RIGHT = 1
}

public enum CutDirection
{
    TOP = 1,
    BOTTOM = 0,
    LEFT = 2,
    RIGHT = 3,
    TOPLEFT = 6,
    TOPRIGHT = 7,
    BOTTOMLEFT = 4,
    BOTTOMRIGHT = 5,
    NONDIRECTION = 8
}

public enum ObstacleType
{
    WALL = 0,
    CEILING = 1
}