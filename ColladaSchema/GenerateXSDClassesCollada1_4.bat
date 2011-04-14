@echo off
setlocal
set sdkbin="C:\Program Files (x86)\Microsoft Visual Studio 8\SDK\v2.0\Bin"

if /i "%1" == "" goto all

:all
:end


%sdkbin%\xsd collada_schema_1_4.xsd /classes /outputdir:generated

sleep 20

