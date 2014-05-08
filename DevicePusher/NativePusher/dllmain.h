// dllmain.h : Declaration of module class.

class CNativePusherModule : public ATL::CAtlDllModuleT< CNativePusherModule >
{
public :
	DECLARE_LIBID(LIBID_NativePusherLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_NATIVEPUSHER, "{1EC8B427-5526-4C76-BDCC-0196FA243893}")
};

extern class CNativePusherModule _AtlModule;
