using RogueEssence.Ground;
using System;

namespace RogueEssence.Script
{
    /// <summary>
    /// Handles script interactions with character AI
    /// </summary>
    class ScriptAI : ILuaEngineComponent
    {

        /// <summary>
        /// Assigns the given scripted AI class to the specified GroundChar.
        /// </summary>
        /// <param name="ch">The ground character to assign AI to.</param>
        /// <param name="classpath">The path to the Lua AI class.</param>
        /// <param name="args">Additional arguments to pass to the AI constructor.</param>
        public void SetCharacterAI(GroundChar ch, string classpath, params object[] args)
        {
            try
            {
                //!#TODO: Could add a check to see if the class can be loaded and return a boolean result?
                ch.SetAI(new GroundScriptedAI(classpath, args));
            }
            catch (Exception ex)
            {
                DiagManager.Instance.LogError(ex, DiagManager.Instance.DevMode);
            }
        }

        /// <summary>
        /// Disables a given ground character's AI processing until it is enabled again.
        /// </summary>
        /// <param name="ch">The ground character whose AI should be disabled.</param>
        public void DisableCharacterAI(GroundChar ch)
        {
            try
            {
                ch.AIEnabled = false;
            }
            catch(Exception ex)
            {
                DiagManager.Instance.LogError(ex, DiagManager.Instance.DevMode);
            }
        }

        /// <summary>
        /// Enables a given ground character's AI processing if it is currently disabled.
        /// </summary>
        /// <param name="ch">The ground character whose AI should be enabled.</param>
        public void EnableCharacterAI(GroundChar ch)
        {
            try
            {
                ch.AIEnabled = true;
            }
            catch (Exception ex)
            {
                DiagManager.Instance.LogError(ex, DiagManager.Instance.DevMode);
            }
        }

        /// <summary>
        /// Sets the AI state for a given ground character.
        /// </summary>
        /// <param name="ch">The ground character to modify.</param>
        /// <param name="state">The name of the AI state to transition to.</param>
        public void SetAIState(GroundChar ch, string state)
        {
            try
            {
                ch.SetAIState(state);
            }
            catch (Exception ex)
            {
                DiagManager.Instance.LogError(ex, DiagManager.Instance.DevMode);
            }
        }


        /// <summary>
        /// Sets up any Lua function wrappers for this component.
        /// </summary>
        /// <param name="state">The Lua engine state.</param>
        public override void SetupLuaFunctions(LuaEngine state)
        {
        }
    }
}
