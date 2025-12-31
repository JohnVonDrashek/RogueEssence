using RogueEssence.Content;
using System;
using System.Collections.Generic;
using System.Text;

namespace RogueEssence.Dev.ViewModels
{
    /// <summary>
    /// ViewModel for the Sprites tab in the developer form.
    /// Provides access to editors for various sprite and graphic asset types.
    /// </summary>
    public class DevTabSpritesViewModel : ViewModelBase
    {
        /// <summary>
        /// Opens the species sprite editor.
        /// </summary>
        public void btnEditSprites_Click()
        {
            SpeciesEditViewModel mv = new SpeciesEditViewModel();
            Views.SpeciesEditForm editForm = new Views.SpeciesEditForm();
            mv.LoadFormDataEntries(true, editForm);
            editForm.DataContext = mv;
            editForm.Show();
        }

        /// <summary>
        /// Opens the species portrait editor.
        /// </summary>
        public void btnEditPortraits_Click()
        {
            SpeciesEditViewModel mv = new SpeciesEditViewModel();
            Views.SpeciesEditForm editForm = new Views.SpeciesEditForm();
            mv.LoadFormDataEntries(false, editForm);
            editForm.DataContext = mv;
            editForm.Show();
        }

        /// <summary>
        /// Opens the particle effects editor.
        /// </summary>
        public void btnEditParticles_Click()
        {
            AnimEditViewModel mv = new AnimEditViewModel();
            Views.AnimEditForm editForm = new Views.AnimEditForm();
            mv.LoadDataEntries(GraphicsManager.AssetType.Particle, editForm);
            editForm.DataContext = mv;
            editForm.Show();
        }

        /// <summary>
        /// Opens the beam effects editor.
        /// </summary>
        public void btnEditBeams_Click()
        {
            BeamEditViewModel mv = new BeamEditViewModel();
            Views.AnimEditForm editForm = new Views.AnimEditForm();
            mv.LoadDataEntries(GraphicsManager.AssetType.Beam, editForm);
            editForm.DataContext = mv;
            editForm.Show();
        }

        /// <summary>
        /// Opens the background graphics editor.
        /// </summary>
        public void btnEditBGs_Click()
        {
            AnimEditViewModel mv = new AnimEditViewModel();
            Views.AnimEditForm editForm = new Views.AnimEditForm();
            mv.LoadDataEntries(GraphicsManager.AssetType.BG, editForm);
            editForm.DataContext = mv;
            editForm.Show();
        }

        /// <summary>
        /// Opens the emote icons editor.
        /// </summary>
        public void btnEditEmotes_Click()
        {
            AnimEditViewModel mv = new AnimEditViewModel();
            Views.AnimEditForm editForm = new Views.AnimEditForm();
            mv.LoadDataEntries(GraphicsManager.AssetType.Icon, editForm);
            editForm.DataContext = mv;
            editForm.Show();
        }

        /// <summary>
        /// Opens the tileset editor.
        /// </summary>
        public void btnEditTiles_Click()
        {
            TilesetEditViewModel mv = new TilesetEditViewModel();
            Views.TilesetEditForm editForm = new Views.TilesetEditForm();
            mv.LoadDataEntries(editForm);
            editForm.DataContext = mv;
            editForm.Show();
        }

        /// <summary>
        /// Opens the item graphics editor.
        /// </summary>
        public void btnEditItems_Click()
        {
            AnimEditViewModel mv = new AnimEditViewModel();
            Views.AnimEditForm editForm = new Views.AnimEditForm();
            mv.LoadDataEntries(GraphicsManager.AssetType.Item, editForm);
            editForm.DataContext = mv;
            editForm.Show();
        }

        /// <summary>
        /// Opens the object graphics editor.
        /// </summary>
        public void btnEditObjects_Click()
        {
            AnimEditViewModel mv = new AnimEditViewModel();
            Views.AnimEditForm editForm = new Views.AnimEditForm();
            mv.LoadDataEntries(GraphicsManager.AssetType.Object, editForm);
            editForm.DataContext = mv;
            editForm.Show();
        }
    }
}
