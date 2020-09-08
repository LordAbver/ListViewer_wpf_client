using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ListViewer_wpf_client.MaterialServiceReference;
namespace ListViewer_wpf_client.ServicesCallbacks
{
    class MaterialServiceCallback:IMaterialServiceCallback
    {
        public void OnDatabaseChanged(DbChangeInfoDTO change)
        {
           //
            var a = 3;
        }

        public void OnVerifyList(VerifyListDTO data)
        {

            var a = 3;
        }

        public void OnDBsearch(MaterialSearchDTO[] data, Guid operationId)
        {
            //
            var a = 3;
        }

        public void OnGetStorageCollection(CacheDTO[] data, Guid operationId)
        {
            throw new NotImplementedException();
        }
    }
}
