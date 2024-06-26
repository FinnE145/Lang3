using Microsoft.Language.Xml;
using System.Collections.Concurrent;

namespace Lang3Server;
class BufferManager
{
    private ConcurrentDictionary<string, Microsoft.Language.Xml.Buffer> _buffers = new ConcurrentDictionary<string, Microsoft.Language.Xml.Buffer>();

    public void UpdateBuffer(string documentPath, Microsoft.Language.Xml.Buffer buffer)
    {
        _buffers.AddOrUpdate(documentPath, buffer, (k, v) => buffer);
    }

    public Microsoft.Language.Xml.Buffer? GetBuffer(string documentPath)
    {
        return _buffers.TryGetValue(documentPath, out var buffer) ? buffer : null;
    }
}