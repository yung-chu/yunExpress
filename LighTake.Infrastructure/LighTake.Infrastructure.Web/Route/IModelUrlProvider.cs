using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LighTake.Infrastructure.Web.Route
{
  public interface IModelUrlProvider<T>
  {
    T GetModel( string path );

    string GetPath( T model );
  }

}
