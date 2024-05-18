using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeWhen.Infrastructure.Utilities
{
    public interface ISupabaseUtilities : Supabase.Interfaces.ISupabaseClient<Supabase.Gotrue.User, Supabase.Gotrue.Session, Supabase.Realtime.RealtimeSocket, Supabase.Realtime.RealtimeChannel, Supabase.Storage.Bucket, Supabase.Storage.FileObject>
    {

    }
}