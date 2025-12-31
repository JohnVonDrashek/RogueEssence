using System;
using System.Collections.Generic;
using System.IO;
using RogueElements;
using RogueEssence.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueEssence.Data;
using RogueEssence.Menu;
using RogueEssence.Dungeon;
using RogueEssence.Ground;
using RogueEssence.Script;
using RogueEssence.Dev;
using System.Linq;

namespace RogueEssence
{
    /// <summary>
    /// Abstract base class for music transition effects.
    /// </summary>
    public abstract class MusicEffect
    {
        /// <summary>
        /// Gets whether the music effect has finished.
        /// </summary>
        public abstract bool Finished { get; }

        /// <summary>
        /// Updates the music effect state.
        /// </summary>
        /// <param name="elapsedTime">Time elapsed since last update.</param>
        /// <param name="musicFadeFraction">Reference to the music fade fraction to modify.</param>
        /// <param name="crossFadeFraction">Dictionary of cross-fade fractions per song file.</param>
        public abstract void Update(FrameTick elapsedTime, ref float musicFadeFraction, Dictionary<string, float> crossFadeFraction);
    }

    /// <summary>
    /// Abstract base class for music fade transition effects.
    /// Provides common functionality for fading music in and out.
    /// </summary>
    public abstract class MusicFadeEffect : MusicEffect
    {
        /// <summary>
        /// The next song to play after the fade.
        /// </summary>
        public string NextSong;

        /// <summary>
        /// The remaining time for the fade effect.
        /// </summary>
        public FrameTick MusicFadeTime;

        /// <summary>
        /// The total duration of the fade effect.
        /// </summary>
        public int MusicFadeTotal;

        /// <summary>
        /// Gets whether the fade effect has finished.
        /// </summary>
        public override bool Finished { get { return NextSong == null; } }

        /// <summary>
        /// Initializes a new instance of the MusicFadeEffect class.
        /// </summary>
        /// <param name="newBGM">The new background music to play.</param>
        /// <param name="fadeTime">The fade time duration.</param>
        /// <param name="fadeTotal">The total fade duration.</param>
        public MusicFadeEffect(string newBGM, FrameTick fadeTime, int fadeTotal)
        {
            NextSong = newBGM;
            MusicFadeTime = fadeTime;
            MusicFadeTotal = fadeTotal;
        }


        protected void onMusicChange(string newBGM)
        {
            if (newBGM.Length > 0)
            {
                string name = "";
                string originName = "";
                string origin = "";
                string artist = "";
                string spoiler = "";
                string fileName = PathMod.ModPath(GraphicsManager.MUSIC_PATH + newBGM);
                if (File.Exists(fileName))
                {
                    LoopedSong song = new LoopedSong(fileName);
                    name = song.Name;
                    if (song.Tags.ContainsKey("TITLE"))
                        originName = song.Tags["TITLE"][0];
                    if (song.Tags.ContainsKey("ALBUM"))
                        origin = song.Tags["ALBUM"][0];
                    if (song.Tags.ContainsKey("ARTIST"))
                        artist = song.Tags["ARTIST"][0];
                    if (song.Tags.ContainsKey("SPOILER"))
                        spoiler = song.Tags["SPOILER"][0];
                    LuaEngine.Instance.OnMusicChange(name, originName, origin, artist, spoiler);
                }
            }
        }
    }

    /// <summary>
    /// Music effect that cross-fades between songs in the same family.
    /// Smoothly transitions volume between tracks without stopping playback.
    /// </summary>
    public class MusicCrossFadeEffect : MusicFadeEffect
    {
        /// <summary>
        /// Initializes a new instance of the MusicCrossFadeEffect class.
        /// </summary>
        /// <param name="newBGM">The new background music to cross-fade to.</param>
        /// <param name="fadeTime">The fade time duration.</param>
        /// <param name="fadeTotal">The total fade duration.</param>
        public MusicCrossFadeEffect(string newBGM, FrameTick fadeTime, int fadeTotal)
            : base(newBGM, fadeTime, fadeTotal)
        {
        }

        /// <summary>
        /// Updates the cross-fade effect, adjusting volume levels between songs.
        /// </summary>
        /// <param name="elapsedTime">Time elapsed since last update.</param>
        /// <param name="musicFadeFraction">Reference to the music fade fraction.</param>
        /// <param name="crossFadeFraction">Dictionary of cross-fade fractions per song file.</param>
        public override void Update(FrameTick elapsedTime, ref float musicFadeFraction, Dictionary<string, float> crossFadeFraction)
        {
            if (NextSong != null)
            {
                MusicFadeTime -= elapsedTime;
                if (MusicFadeTime <= FrameTick.Zero)
                {
                    GameManager.Instance.Song = NextSong;
                    NextSong = null;

                    foreach (string songName in GameManager.Instance.SongFamily.Keys)
                    {
                        float defaultVol = (GameManager.Instance.Song == songName) ? 1f : 0f;
                        crossFadeFraction[GameManager.Instance.SongFamily[songName]] = defaultVol;
                    }
                }
                else
                {
                    foreach (string songName in GameManager.Instance.SongFamily.Keys)
                    {
                        float defaultVol = 0;
                        if (GameManager.Instance.Song == songName)
                            defaultVol = MusicFadeTime.FractionOf(MusicFadeTotal);
                        else if (NextSong == songName)
                            defaultVol = 1f - MusicFadeTime.FractionOf(MusicFadeTotal);
                        crossFadeFraction[GameManager.Instance.SongFamily[songName]] = defaultVol;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Music effect that fades out the current music and starts a new song family.
    /// Stops the current music completely before starting new tracks.
    /// </summary>
    public class MusicFadeOutEffect : MusicFadeEffect
    {
        /// <summary>
        /// The set of song file names that belong to the new song family.
        /// </summary>
        public HashSet<string> Family;

        /// <summary>
        /// Initializes a new instance of the MusicFadeOutEffect class.
        /// </summary>
        /// <param name="newBGM">The new background music to play.</param>
        /// <param name="family">The set of songs in the new song family.</param>
        /// <param name="fadeTime">The fade time duration.</param>
        /// <param name="fadeTotal">The total fade duration.</param>
        public MusicFadeOutEffect(string newBGM, HashSet<string> family, FrameTick fadeTime, int fadeTotal)
            : base(newBGM, fadeTime, fadeTotal)
        {
            Family = family;
        }

        /// <summary>
        /// Updates the fade-out effect, reducing volume and starting new music when complete.
        /// </summary>
        /// <param name="elapsedTime">Time elapsed since last update.</param>
        /// <param name="musicFadeFraction">Reference to the music fade fraction.</param>
        /// <param name="crossFadeFraction">Dictionary of cross-fade fractions per song file.</param>
        public override void Update(FrameTick elapsedTime, ref float musicFadeFraction, Dictionary<string, float> crossFadeFraction)
        {
            if (NextSong != null)
            {
                MusicFadeTime -= elapsedTime;
                if (MusicFadeTime <= FrameTick.Zero)
                {
                    string moddedPath = PathMod.ModPath(GraphicsManager.MUSIC_PATH + NextSong);
                    if (File.Exists(moddedPath))
                    {
                        GameManager.Instance.Song = NextSong;
                        Dictionary<string, string> family = new Dictionary<string, string>();
                        List<string> fileList = new List<string>();
                        foreach (string familyName in Family)
                        {
                            string file = PathMod.ModPath(GraphicsManager.MUSIC_PATH + familyName);
                            fileList.Add(file);
                            family.Add(familyName, file);
                        }
                        GameManager.Instance.SongFamily = family;
                        onMusicChange(NextSong);
                        SoundManager.PlayBGM(moddedPath, fileList.ToArray());
                    }
                    else
                    {
                        GameManager.Instance.Song = "";
                        GameManager.Instance.SongFamily = new Dictionary<string, string>();
                        SoundManager.PlayBGM(GameManager.Instance.Song, new string[0]);
                    }
                    NextSong = null;

                    crossFadeFraction.Clear();
                    foreach (string songName in GameManager.Instance.SongFamily.Keys)
                    {
                        float defaultVol = (GameManager.Instance.Song == songName) ? 1f : 0f;
                        crossFadeFraction[GameManager.Instance.SongFamily[songName]] = defaultVol;
                    }
                }
                else
                    musicFadeFraction *= MusicFadeTime.FractionOf(MusicFadeTotal);
            }
        }
    }

    /// <summary>
    /// Music effect that plays a fanfare sound over the current music.
    /// Fades music out, plays the fanfare, then fades music back in.
    /// </summary>
    public class FanfareEffect : MusicEffect
    {
        /// <summary>
        /// Duration in frames for the initial music fade-out.
        /// </summary>
        public const int FANFARE_FADE_START = 3;

        /// <summary>
        /// Duration in frames for the music fade-in after fanfare.
        /// </summary>
        public const int FANFARE_FADE_END = 40;

        /// <summary>
        /// Extra wait time in frames after fanfare playback.
        /// </summary>
        public const int FANFARE_WAIT_EXTRA = 20;

        /// <summary>
        /// Represents the different phases of fanfare playback.
        /// </summary>
        public enum FanfarePhase
        {
            /// <summary>Fanfare is not playing.</summary>
            None,
            /// <summary>Music is fading out before fanfare.</summary>
            PhaseOut,
            /// <summary>Waiting while fanfare plays.</summary>
            Wait,
            /// <summary>Music is fading back in after fanfare.</summary>
            PhaseIn
        }

        /// <summary>
        /// The name of the fanfare sound file to play.
        /// </summary>
        public string Fanfare;

        /// <summary>
        /// The remaining time for the current fanfare phase.
        /// </summary>
        public FrameTick FanfareTime;

        /// <summary>
        /// The current phase of the fanfare effect.
        /// </summary>
        public FanfarePhase CurrentPhase;

        /// <summary>
        /// Gets whether the fanfare effect has finished.
        /// </summary>
        public override bool Finished { get { return CurrentPhase == FanfarePhase.None; } }

        /// <summary>
        /// Initializes a new instance of the FanfareEffect class.
        /// </summary>
        /// <param name="fanfare">The name of the fanfare sound file.</param>
        public FanfareEffect(string fanfare)
        {
            CurrentPhase = FanfarePhase.PhaseOut;
            FanfareTime = FrameTick.FromFrames(FANFARE_FADE_START);
            Fanfare = fanfare;
        }

        /// <summary>
        /// Updates the fanfare effect, managing phase transitions and volume levels.
        /// </summary>
        /// <param name="elapsedTime">Time elapsed since last update.</param>
        /// <param name="musicFadeFraction">Reference to the music fade fraction.</param>
        /// <param name="crossFadeFraction">Dictionary of cross-fade fractions per song file.</param>
        public override void Update(FrameTick elapsedTime, ref float musicFadeFraction, Dictionary<string, float> crossFadeFraction)
        {
            if (CurrentPhase != FanfarePhase.None)
            {
                FanfareTime -= elapsedTime;
                if (CurrentPhase == FanfarePhase.PhaseOut)
                {
                    musicFadeFraction *= FanfareTime.FractionOf(FANFARE_FADE_START);
                    if (FanfareTime <= FrameTick.Zero)
                    {
                        int pauseFrames = 0;
                        if (File.Exists(PathMod.ModPath(GraphicsManager.SOUND_PATH + Fanfare + ".ogg")))
                            pauseFrames = SoundManager.PlaySound(PathMod.ModPath(GraphicsManager.SOUND_PATH + Fanfare + ".ogg"), 1) + FANFARE_WAIT_EXTRA;
                        CurrentPhase = FanfarePhase.Wait;
                        if (FanfareTime < pauseFrames)
                            FanfareTime = FrameTick.FromFrames(pauseFrames);
                    }
                }
                else if (CurrentPhase == FanfarePhase.Wait)
                {
                    musicFadeFraction *= 0;
                    if (FanfareTime <= FrameTick.Zero)
                    {
                        CurrentPhase = FanfarePhase.PhaseIn;
                        FanfareTime = FrameTick.FromFrames(FANFARE_FADE_END);
                    }
                }
                else if (CurrentPhase == FanfarePhase.PhaseIn)
                {
                    musicFadeFraction *= (1f - FanfareTime.FractionOf(FANFARE_FADE_END));
                    if (FanfareTime <= FrameTick.Zero)
                        CurrentPhase = FanfarePhase.None;
                }
            }
        }
    }
}
