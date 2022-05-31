using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
using System.EnterpriseServices;

namespace SeeYouOnCOM
{
    [ComVisible(true)]
    [Guid("EC99D774-C432-469c-8A09-9A42BAF6F790")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]

    public interface ISyncCOM
    {
        void Ping();

        void SyncTMSs(int liDataContext);
        void SyncSystems(int liCodTMS, int liDataContext);
        void SyncMoviAccounts(int liCodTMS, int liDataContext);
        void SyncPhoneBooks(int liCodTMS, int liDataContext);
        void SyncConferences(int liCodTMS, int liDataContext);
        void SyncProvGroups(int liCodTMS, int liDataContext);

        //int SYOConfSave(int liCodTMS, Conference loTMSConf, int liTrId);
        //int SYOConfSave(string lsCodigo, Hashtable lhtVal);
        //void SYOConfDelete(int liCodTMS, Conference loTMSConf, int liTrId);
        //void SYOConfDelete(int liCodRegistro);
        //void SYOConfDelete(int liCodRegistro, int liTrId);
        //int SYOConfPartSave(int liCodConf, Participant loTMSPart);

        //string MCUConfSave(DateTime ldtStart, DateTime ldtEnd, int liDataContext);
        //string MCUConfSave(string lsNumericId, DateTime ldtStart, DateTime ldtEnd, int liDataContext);
        //string MCUConfSave(int liCodMCU, DateTime ldtStart, DateTime ldtEnd, int liDataContext);
        //string MCUConfSave(int liCodMCU, string lsNumericId, DateTime ldtStart, DateTime ldtEnd, int liDataContext);

        //void MCUConfDelete(string lsNumericId, int liDataContext);
        //void MCUConfDelete(int liCodMCU, string lsNumericId, int liDataContext);

        void MCUConfSave(int liCodConf, int liDataContext);
        void MCUConfDelete(int liCodConf, int liDataContext);

        //void MCUPartSave(string lsNumericId, int liCodSystem, int liDataContext);
        //void MCUPartSave(int liCodMCU, string lsNumericId, int liCodSystem, int liDataContext);

        //void MCUPartDelete(string lsNumericId, int liCodSystem, int liDataContext);
        //void MCUPartDelete(int liCodMCU, string lsNumericId, int liCodSystem, int liDataContext);

        void TMSMoviSave(int liCodMoviAccount, int liDataContext);
        void TMSMoviDelete(int liCodMoviAccount, int liDataContext);
        void NotificAsistConf(int liCodConferencia, string lsLang, int liDataContext);

        void EnviaCtaMOVI(int liCodCuentaMovi, string lsLang, string lsStyle, int liDataContext);
        void RemoveCtasMOVI(int liDataContext);

    }
}
