using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Controls;
using RogueElements;
using RogueEssence.Content;
using RogueEssence.Data;
using RogueEssence.Dungeon;
using RogueEssence.Menu;
using RogueEssence.Script;
using RogueEssence.Dev.Views;

namespace RogueEssence.Dev.ViewModels
{
    /// <summary>
    /// ViewModel for the Constants tab in the developer form.
    /// Provides functionality for editing game constants, universal events, strings, and battle effects.
    /// </summary>
    public class DevTabConstantsViewModel : ViewModelBase
    {
        /// <summary>
        /// Opens the editor for start parameters.
        /// </summary>
        public void btnEditStartParams_Click()
        {
            OpenItem<StartParams>("Start Params", DataManager.Instance.Start, (obj) => {
                DataManager.Instance.Start = obj;
                DataManager.Instance.SaveStartParams();
            });
        }

        /// <summary>
        /// Opens the editor for universal base effects.
        /// </summary>
        public void btnEditUniversal_Click()
        {
            OpenItem<UniversalBaseEffect>("Universal Event", (UniversalBaseEffect)DataManager.Instance.UniversalEvent, (obj) => {
                DataManager.Instance.UniversalEvent = obj;
                DataManager.SaveData(obj, DataManager.DATA_PATH, "Universal", DataManager.DATA_EXT);
            });
        }
        /// <summary>
        /// Saves the universal data as a complete file.
        /// </summary>
        public async void mnuUniversalFile_Click()
        {
            DevForm parent = (DevForm)DiagManager.Instance.DevEditor;
            if (DataManager.GetDataModStatus(DataManager.DATA_PATH, "Universal", DataManager.DATA_EXT) == DataManager.ModStatus.Base)
            {
                await MessageBox.Show(parent, "Universal data must have saved edits first!", "Error", MessageBox.MessageBoxButtons.Ok);
                return;
            }

            DataManager.SaveData(DataManager.Instance.UniversalEvent, DataManager.DATA_PATH, "Universal", DataManager.DATA_EXT, DataManager.SavePolicy.File);

            await MessageBox.Show(parent, "Universal is now saved as a file.", "Complete", MessageBox.MessageBoxButtons.Ok);
        }
        /// <summary>
        /// Saves the universal data as a diff/patch file.
        /// </summary>
        public async void mnuUniversalDiff_Click()
        {
            DevForm parent = (DevForm)DiagManager.Instance.DevEditor;
            if (DataManager.GetDataModStatus(DataManager.DATA_PATH, "Universal", DataManager.DATA_EXT) == DataManager.ModStatus.Base)
            {
                await MessageBox.Show(parent, "Universal data must have saved edits first!", "Error", MessageBox.MessageBoxButtons.Ok);
                return;
            }

            //you can't make a diff for the base game!
            DataManager.SaveData(DataManager.Instance.UniversalEvent, DataManager.DATA_PATH, "Universal", DataManager.DATA_EXT, DataManager.SavePolicy.Diff);

            if (DataManager.GetDataModStatus(DataManager.DATA_PATH, "Universal", DataManager.DATA_EXT) == DataManager.ModStatus.Base)
                await MessageBox.Show(parent, "Modded Universal was identical to base. Unneeded patch removed.", "Complete", MessageBox.MessageBoxButtons.Ok);
            else
                await MessageBox.Show(parent, "Universal is now saved as a patch.", "Complete", MessageBox.MessageBoxButtons.Ok);
        }

        /// <summary>
        /// Opens the strings editor for base game strings.
        /// </summary>
        public void btnEditStrings_Click()
        {
            StringsEditViewModel mv = new StringsEditViewModel();
            Views.StringsEditForm editForm = new Views.StringsEditForm();
            mv.LoadStringEntries(false, editForm);
            editForm.DataContext = mv;
            editForm.Show();
        }

        /// <summary>
        /// Opens the strings editor for extended strings.
        /// </summary>
        public void btnEditStringsEx_Click()
        {
            StringsEditViewModel mv = new StringsEditViewModel();
            Views.StringsEditForm editForm = new Views.StringsEditForm();
            mv.LoadStringEntries(true, editForm);
            editForm.DataContext = mv;
            editForm.Show();
        }



        /// <summary>
        /// Opens the editor for heal battle effects.
        /// </summary>
        public void btnEditHeal_Click()
        {
            OpenItem<BattleFX>("Heal FX", DataManager.Instance.HealFX, (fx) => {
                DataManager.Instance.HealFX = fx;
                DataManager.SaveData(fx, DataManager.FX_PATH, "Heal", DataManager.DATA_EXT);
            });
        }

        /// <summary>
        /// Opens the editor for restore charge battle effects.
        /// </summary>
        public void btnEditRestoreCharge_Click()
        {
            OpenItem<BattleFX>("Restore Charge FX", DataManager.Instance.RestoreChargeFX, (fx) => {
                DataManager.Instance.RestoreChargeFX = fx;
                DataManager.SaveData(fx, DataManager.FX_PATH, "RestoreCharge", DataManager.DATA_EXT);
            });
        }

        /// <summary>
        /// Opens the editor for lose charge battle effects.
        /// </summary>
        public void btnEditLoseCharge_Click()
        {
            OpenItem<BattleFX>("Lose Charge FX", DataManager.Instance.LoseChargeFX, (fx) => {
                DataManager.Instance.LoseChargeFX = fx;
                DataManager.SaveData(fx, DataManager.FX_PATH, "LoseCharge", DataManager.DATA_EXT);
            });
        }

        /// <summary>
        /// Opens the editor for no charge emote effects.
        /// </summary>
        public void btnEditNoCharge_Click()
        {
            OpenItem<EmoteFX>("No Charge FX", DataManager.Instance.NoChargeFX, (fx) => {
                DataManager.Instance.NoChargeFX = fx;
                DataManager.SaveData(fx, DataManager.FX_PATH, "NoCharge", DataManager.DATA_EXT);
            });
        }

        /// <summary>
        /// Opens the editor for element battle effects.
        /// </summary>
        public void btnEditElement_Click()
        {
            OpenItem<BattleFX>("Element FX", DataManager.Instance.ElementFX, (fx) => {
                DataManager.Instance.ElementFX = fx;
                DataManager.SaveData(fx, DataManager.FX_PATH, "Element", DataManager.DATA_EXT);
            });
        }

        /// <summary>
        /// Opens the editor for intrinsic battle effects.
        /// </summary>
        public void btnEditIntrinsic_Click()
        {
            OpenItem<BattleFX>("Intrinsic FX", DataManager.Instance.IntrinsicFX, (fx) => {
                DataManager.Instance.IntrinsicFX = fx;
                DataManager.SaveData(fx, DataManager.FX_PATH, "Intrinsic", DataManager.DATA_EXT);
            });
        }

        /// <summary>
        /// Opens the editor for send home battle effects.
        /// </summary>
        public void btnEditSendHome_Click()
        {
            OpenItem<BattleFX>("Send Home FX", DataManager.Instance.SendHomeFX, (fx) => {
                DataManager.Instance.SendHomeFX = fx;
                DataManager.SaveData(fx, DataManager.FX_PATH, "SendHome", DataManager.DATA_EXT);
            });
        }

        /// <summary>
        /// Opens the editor for item lost battle effects.
        /// </summary>
        public void btnEditItemLost_Click()
        {
            OpenItem<BattleFX>("Item Lost FX", DataManager.Instance.ItemLostFX, (fx) => {
                DataManager.Instance.ItemLostFX = fx;
                DataManager.SaveData(fx, DataManager.FX_PATH, "ItemLost", DataManager.DATA_EXT);
            });
        }

        /// <summary>
        /// Opens the editor for warp battle effects.
        /// </summary>
        public void btnEditWarp_Click()
        {
            OpenItem<BattleFX>("Warp FX", DataManager.Instance.WarpFX, (fx) => {
                DataManager.Instance.WarpFX = fx;
                DataManager.SaveData(fx, DataManager.FX_PATH, "Warp", DataManager.DATA_EXT);
            });
        }

        /// <summary>
        /// Opens the editor for knockback battle effects.
        /// </summary>
        public void btnEditKnockback_Click()
        {
            OpenItem<BattleFX>("Knockback FX", DataManager.Instance.KnockbackFX, (fx) => {
                DataManager.Instance.KnockbackFX = fx;
                DataManager.SaveData(fx, DataManager.FX_PATH, "Knockback", DataManager.DATA_EXT);
            });
        }

        /// <summary>
        /// Opens the editor for jump battle effects.
        /// </summary>
        public void btnEditJump_Click()
        {
            OpenItem<BattleFX>("Jump FX", DataManager.Instance.JumpFX, (fx) => {
                DataManager.Instance.JumpFX = fx;
                DataManager.SaveData(fx, DataManager.FX_PATH, "Jump", DataManager.DATA_EXT);
            });
        }

        /// <summary>
        /// Opens the editor for throw battle effects.
        /// </summary>
        public void btnEditThrow_Click()
        {
            OpenItem<BattleFX>("Throw FX", DataManager.Instance.ThrowFX, (fx) => { DataManager.Instance.ThrowFX = fx;
                DataManager.SaveData(fx, DataManager.FX_PATH, "Throw", DataManager.DATA_EXT);
            });
        }

        private delegate void SaveFX<T>(T obj);
        private void OpenItem<T>(string name, T data, SaveFX<T> saveOp)
        {
            lock (GameBase.lockObj)
            {
                Views.DataEditForm editor = new Views.DataEditRootForm();
                editor.Title = DataEditor.GetWindowTitle("", name, data, data.GetType());
                DataEditor.LoadDataControls("", data, editor);
                editor.SelectedOKEvent += async () =>
                {
                    lock (GameBase.lockObj)
                    {
                        object obj = data;
                        DataEditor.SaveDataControls(ref obj, editor.ControlPanel, new Type[0]);
                        saveOp((T)obj);
                        return true;
                    }
                };

                editor.Show();

            }
        }

    }
}
