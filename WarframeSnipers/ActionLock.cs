using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarframeDPSTool
{
    class ActionLock : IDisposable
    {
        private readonly Action _OnComplete;
        public ActionLock(Action pOnComplete)
        {
            this._OnComplete = pOnComplete;
        }
        public void Dispose()
        {
            if (this._OnComplete != null)
            {
                this._OnComplete.Invoke();
            }
        }
    }
}
