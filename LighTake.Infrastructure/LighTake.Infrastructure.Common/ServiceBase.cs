using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LighTake.Infrastructure.Common
{
    /// <summary>
    /// Service层基类，用于释放对象
    /// </summary>
   public abstract class ServiceBase:IDisposable
    {
       public IList<IDisposable> DisposableObjects { get; private set; }

       public ServiceBase()
       {
           this.DisposableObjects=new List<IDisposable>();
       }


       protected void AddDisposableObject(object obj)
       {
           var disposable = obj as IDisposable;
           if (null != disposable)
           {
               this.DisposableObjects.Add(disposable);
           }
       }
       protected void AddDisposableObject(params object[] objects)
       {
           foreach (var obj in objects)
           {
               var disposable = obj as IDisposable;
               if (null != disposable)
               {
                   this.DisposableObjects.Add(disposable);
               }
           }
       }
       public void Dispose()
        {
           foreach (var obj in DisposableObjects)
           {
               if (null != obj)
               {
                   obj.Dispose();
               }
           }
        }
    }
}
