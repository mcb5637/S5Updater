
if UserScript then
    return
end

UserScript = {}

function UserScript.OverrideWeathersets()
    UserScript.OverrideWeathersetNormal()
    UserScript.OverrideWeathersetEvelance()
    UserScript.OverrideWeathersetMedi()
    UserScript.OverrideWeathersetHigh()
    UserScript.OverrideWeathersetMoor()
    UserScript.OverrideWeathersetSteppe()
    UserScript.WeathersetsFixed = true
end

function UserScript.OverrideWeathersetNormal()
    function WeatherSets_SetupNormal(_ID, _Rain, _Snow)
        Display.GfxSetSetSkyBox(_ID, 0.0, 1.0, "YSkyBox02")
        if _Rain ~= nil then
            Display.GfxSetSetRainEffectStatus(_ID, 0.0, 1.0, _Rain)
        else
            Display.GfxSetSetRainEffectStatus(_ID, 0.0, 1.0, 0)
        end
        Display.GfxSetSetSnowStatus(_ID, 0, 1.0, 0)
        if _Snow ~= nil then
            Display.GfxSetSetSnowEffectStatus(_ID, 0.0, 0.8, _Snow)
        else
            Display.GfxSetSetSnowEffectStatus(_ID, 0.0, 0.8, 0)
        end
        Display.GfxSetSetFogParams(_ID, 0.0, 1.0, 1, 152, 172, 182, 5000, 28000)
        Display.GfxSetSetLightParams(_ID, 0.0, 1.0, 40, -15, -50, 120, 110, 110, 205, 204, 180)
    end

    function WeatherSets_SetupSnow(_ID, _Rain, _Snow)
        Display.GfxSetSetSkyBox(_ID, 0.0, 1.0, "YSkyBox01")
        if _Rain ~= nil then
            Display.GfxSetSetRainEffectStatus(_ID, 0.0, 1.0, _Rain)
        else
            Display.GfxSetSetRainEffectStatus(_ID, 0.0, 1.0, 0)
        end
        Display.GfxSetSetSnowStatus(_ID, 0, 1.0, 1)
        if _Snow ~= nil then
            Display.GfxSetSetSnowEffectStatus(_ID, 0.0, 0.8, _Snow)
        else
            Display.GfxSetSetSnowEffectStatus(_ID, 0.0, 0.8, 1)
        end
        Display.GfxSetSetFogParams(_ID, 0.0, 1.0, 1, 152, 172, 182, 3000, 19000)
        Display.GfxSetSetLightParams(_ID, 0.0, 1.0, 40, -15, -75, 116, 144, 164, 255, 234, 202)
    end

    function WeatherSets_SetupRain(_ID, _Rain, _Snow)
        Display.GfxSetSetSkyBox(_ID, 0.0, 1.0, "YSkyBox04")
        if _Rain ~= nil then
            Display.GfxSetSetRainEffectStatus(_ID, 0.0, 1.0, _Rain)
        else
            Display.GfxSetSetRainEffectStatus(_ID, 0.0, 1.0, 1)
        end
        Display.GfxSetSetSnowStatus(_ID, 0, 1.0, 0)
        if _Snow ~= nil then
            Display.GfxSetSetSnowEffectStatus(_ID, 0.0, 0.8, _Snow)
        else
            Display.GfxSetSetSnowEffectStatus(_ID, 0.0, 0.8, 0)
        end
        Display.GfxSetSetFogParams(_ID, 0.0, 1.0, 1, 102, 132, 142, 3000, 19000)
        Display.GfxSetSetLightParams(_ID, 0.0, 1.0, 40, -15, -50, 120, 110, 110, 205, 204, 180)
    end
end

function UserScript.OverrideWeathersetEvelance()
    function WeatherSets_SetupEvelance(_ID)
        Display.GfxSetSetSkyBox(_ID, 0.0, 1.0, "YSkyBox07")
        Display.GfxSetSetSnowStatus(_ID, 0, 1.0, 0)
        Display.GfxSetSetSnowEffectStatus(_ID, 0.0, 0.8, 0)
        Display.GfxSetSetFogParams(_ID, 0.0, 1.0, 1, 38, 48, 58, 4000, 19500)
        Display.GfxSetSetLightParams(_ID, 0.0, 1.0, 40, -15, -50, 136, 144, 144, 128, 104, 72)
    end

    function WeatherSets_SetupEvelanceSnow(_ID)
        Display.GfxSetSetSkyBox(_ID, 0.0, 1.0, "YSkyBox01")
        Display.GfxSetSetSnowStatus(_ID, 0, 1.0, 1)
        Display.GfxSetSetSnowEffectStatus(_ID, 0.0, 0.8, 1)
        Display.GfxSetSetFogParams(_ID, 0.0, 1.0, 1, 108, 128, 138, 2000, 19500)
        Display.GfxSetSetLightParams(_ID, 0.0, 1.0, 40, -15, -50, 116, 144, 164, 255, 234, 202)
    end

    function WeatherSets_SetupEvelanceRain(_ID)
        Display.GfxSetSetSkyBox(_ID, 0.0, 1.0, "YSkyBox04")
        Display.GfxSetSetRainEffectStatus(_ID, 0.0, 1.0, 1)
        Display.GfxSetSetSnowStatus(_ID, 0, 1.0, 0)
        Display.GfxSetSetSnowEffectStatus(_ID, 0.0, 0.8, 0)
        Display.GfxSetSetFogParams(_ID, 0.0, 1.0, 1, 38, 58, 68, 4000, 19000)
        Display.GfxSetSetLightParams(_ID, 0.0, 1.0, 40, -15, -50, 136, 144, 144, 128, 104, 72)
    end
end

function UserScript.OverrideWeathersetMedi()
    function WeatherSets_SetupMediterranean(_ID)
        Display.GfxSetSetSkyBox(_ID, 0.0, 1.0, "YSkyBox03")
        Display.GfxSetSetSnowStatus(_ID, 0, 1.0, 0)
        Display.GfxSetSetSnowEffectStatus(_ID, 0.0, 0.8, 0)
        Display.GfxSetSetFogParams(_ID, 0.0, 1.0, 1, 152, 172, 182, 5000, 28000)
        Display.GfxSetSetLightParams(_ID, 0.0, 1.0, 40, -15, -50, 120, 110, 110, 255, 254, 230)
    end

    function WeatherSets_SetupMediterraneanSnow(_ID)
        Display.GfxSetSetSkyBox(_ID, 0.0, 1.0, "YSkyBox01")
        Display.GfxSetSetSnowStatus(_ID, 0, 1.0, 1)
        Display.GfxSetSetSnowEffectStatus(_ID, 0.0, 0.8, 1)
        Display.GfxSetSetFogParams(_ID, 0.0, 1.0, 1, 152, 172, 182, 4000, 19000)
        Display.GfxSetSetLightParams(_ID, 0.0, 1.0, 40, -15, -75, 100, 110, 110, 250, 250, 250)
    end

    function WeatherSets_SetupMediterraneanRain(_ID)
        Display.GfxSetSetSkyBox(_ID, 0.0, 1.0, "YSkyBox04")
        Display.GfxSetSetRainEffectStatus(_ID, 0.0, 1.0, 1)
        Display.GfxSetSetSnowStatus(_ID, 0, 1.0, 0)
        Display.GfxSetSetSnowEffectStatus(_ID, 0.0, 0.8, 0)
        Display.GfxSetSetFogParams(_ID, 0.0, 1.0, 1, 102, 132, 142, 3500, 19500)
        Display.GfxSetSetLightParams(_ID, 0.0, 1.0, 40, -15, -50, 120, 110, 110, 255, 254, 230)
    end
end

function UserScript.OverrideWeathersetHigh()
    function WeatherSets_SetupHighland(_ID)
        Display.GfxSetSetSkyBox(_ID, 0.0, 1.0, "YSkyBox05")
        Display.GfxSetSetSnowStatus(_ID, 0, 1.0, 0)
        Display.GfxSetSetSnowEffectStatus(_ID, 0.0, 0.8, 0)
        Display.GfxSetSetFogParams(_ID, 0.0, 1.0, 1, 152, 172, 182, 5000, 28000)
        Display.GfxSetSetLightParams(_ID, 0.0, 1.0, 40, -15, -50, 120, 110, 110, 255, 254, 230)
    end

    function WeatherSets_SetupHighlandSnow(_ID)
        Display.GfxSetSetSkyBox(_ID, 0.0, 1.0, "YSkyBox01")
        Display.GfxSetSetSnowStatus(_ID, 0, 1.0, 1)
        Display.GfxSetSetSnowEffectStatus(_ID, 0.0, 0.8, 1)
        Display.GfxSetSetFogParams(_ID, 0.0, 1.0, 1, 152, 172, 182, 4000, 19000)
        Display.GfxSetSetLightParams(_ID, 0.0, 1.0, 40, -15, -75, 100, 110, 110, 250, 250, 250)
    end

    function WeatherSets_SetupHighlandRain(_ID)
        Display.GfxSetSetSkyBox(_ID, 0.0, 1.0, "YSkyBox04")
        Display.GfxSetSetRainEffectStatus(_ID, 0.0, 1.0, 1)
        Display.GfxSetSetSnowStatus(_ID, 0, 1.0, 0)
        Display.GfxSetSetSnowEffectStatus(_ID, 0.0, 0.8, 0)
        Display.GfxSetSetFogParams(_ID, 0.0, 1.0, 1, 102, 132, 142, 3000, 19000)
        Display.GfxSetSetLightParams(_ID, 0.0, 1.0, 40, -15, -50, 120, 110, 110, 255, 254, 230)
    end
end

function UserScript.OverrideWeathersetMoor()
    function WeatherSets_SetupMoor(_ID, _Rain, _Snow)
        Display.GfxSetSetSkyBox(_ID, 0.0, 1.0, "YSkyBox02")
        if _Rain ~= nil then
            Display.GfxSetSetRainEffectStatus(_ID, 0.0, 1.0, _Rain)
        else
            Display.GfxSetSetRainEffectStatus(_ID, 0.0, 1.0, 0)
        end
        Display.GfxSetSetSnowStatus(_ID, 0, 1.0, 0)
        if _Snow ~= nil then
            Display.GfxSetSetSnowEffectStatus(_ID, 0.0, 0.8, _Snow)
        else
            Display.GfxSetSetSnowEffectStatus(_ID, 0.0, 0.8, 0)
        end
        Display.GfxSetSetFogParams(_ID, 0.0, 1.0, 1, 171, 164, 114, 4000, 19000)
        Display.GfxSetSetLightParams(_ID, 0.0, 1.0, 40, -15, -36, 100, 100, 100, 185, 164, 142)
    end

    function WeatherSets_SetupMoorSnow(_ID, _Rain, _Snow)
        Display.GfxSetSetSkyBox(_ID, 0.0, 1.0, "YSkyBox01")
        if _Rain ~= nil then
            Display.GfxSetSetRainEffectStatus(_ID, 0.0, 1.0, _Rain)
        else
            Display.GfxSetSetRainEffectStatus(_ID, 0.0, 1.0, 0)
        end
        Display.GfxSetSetSnowStatus(_ID, 0, 1.0, 1)
        if _Snow ~= nil then
            Display.GfxSetSetSnowEffectStatus(_ID, 0.0, 0.8, _Snow)
        else
            Display.GfxSetSetSnowEffectStatus(_ID, 0.0, 0.8, 1)
        end
        Display.GfxSetSetFogParams(_ID, 0.0, 1.0, 1, 151, 164, 114, 5000, 19000)
        Display.GfxSetSetLightParams(_ID, 0.0, 1.0, 40, -15, -75, 116, 144, 164, 255, 234, 202)
    end

    function WeatherSets_SetupMoorRain(_ID, _Rain, _Snow)
        Display.GfxSetSetSkyBox(_ID, 0.0, 1.0, "YSkyBox04")
        if _Rain ~= nil then
            Display.GfxSetSetRainEffectStatus(_ID, 0.0, 1.0, _Rain)
        else
            Display.GfxSetSetRainEffectStatus(_ID, 0.0, 1.0, 1)
        end
        Display.GfxSetSetSnowStatus(_ID, 0, 1.0, 0)

        if _Snow ~= nil then
            Display.GfxSetSetSnowEffectStatus(_ID, 0.0, 0.8, _Snow)
        else
            Display.GfxSetSetSnowEffectStatus(_ID, 0.0, 0.8, 0)
        end
        Display.GfxSetSetFogParams(_ID, 0.0, 1.0, 1, 131, 124, 84, 2000, 19000)
        Display.GfxSetSetLightParams(_ID, 0.0, 1.0, 40, -15, -50, 120, 110, 110, 205, 204, 180)
    end
end

function UserScript.OverrideWeathersetSteppe()
    function WeatherSets_SetupSteppe(_ID)
        Display.GfxSetSetSkyBox(_ID, 0.0, 1.0, "YSkyBox03")
        Display.GfxSetSetSnowStatus(_ID, 0, 1.0, 0)
        Display.GfxSetSetSnowEffectStatus(_ID, 0.0, 0.8, 0)
        Display.GfxSetSetFogParams(_ID, 0.0, 1.0, 1, 170, 172, 172, 7000, 18000)
        Display.GfxSetSetLightParams(_ID, 0.0, 1.0, 40, -15, -25, 167, 167, 209, 255, 226, 226)
    end

    function WeatherSets_SetupSteppeSnow(_ID)
        Display.GfxSetSetSkyBox(_ID, 0.0, 1.0, "YSkyBox01")
        Display.GfxSetSetSnowStatus(_ID, 0, 1.0, 1)
        Display.GfxSetSetSnowEffectStatus(_ID, 0.0, 0.8, 1)
        Display.GfxSetSetFogParams(_ID, 0.0, 1.0, 1, 152, 172, 182, 4000, 19000)
        Display.GfxSetSetLightParams(_ID, 0.0, 1.0, 40, -15, -25, 100, 110, 110, 250, 250, 250)
    end

    function WeatherSets_SetupSteppeRain(_ID)
        Display.GfxSetSetSkyBox(_ID, 0.0, 1.0, "YSkyBox04")
        Display.GfxSetSetRainEffectStatus(_ID, 0.0, 1.0, 1)
        Display.GfxSetSetSnowStatus(_ID, 0, 1.0, 0)
        Display.GfxSetSetSnowEffectStatus(_ID, 0.0, 0.8, 0)
        Display.GfxSetSetFogParams(_ID, 0.0, 1.0, 1, 102, 132, 142, 3500, 19500)
        Display.GfxSetSetLightParams(_ID, 0.0, 1.0, 40, -15, -25, 120, 110, 110, 255, 254, 230)
    end
end

function UserScript.LoadDebug()
    if not mcbPacker then
        Script.Load(GDB.GetString("workspace").."\\s5CommunityLib\\packer\\devLoad.lua")
    end
    mcbPacker.require("comfort/debugtroops")
end

function UserScript.OnWeatherset()
    if UserScriptSettings.Weather and not UserScript.WeathersetsFixed then
        UserScript.OverrideWeathersets()
    end
    if UserScriptSettings.Zoom then
        Camera.ZoomSetFactorMax(UserScriptSettings.Zoom)
    end
    if UserScriptSettings.Debug then
        UserScript.LoadDebug()
        debugtroops.init()
    end
end
function UserScript.OnColor()
    if UserScriptSettings.PlayerColor and not (XNetwork.Manager_DoesExist() == 1 or XNetworkUbiCom.Manager_DoesExist() == 1) then
        Display.SetPlayerColorMapping(GUI.GetPlayerID(), UserScriptSettings.PlayerColor)
    end
end

if InitWeatherGfxSets then
    UserScript.InitWeatherGfxSets = InitWeatherGfxSets
    InitWeatherGfxSets = function()
        UserScript.OnWeatherset()
        UserScript.InitWeatherGfxSets()
    end
end
if Mission_InitWeatherGfxSets then
    UserScript.Mission_InitWeatherGfxSets = Mission_InitWeatherGfxSets
    Mission_InitWeatherGfxSets = function()
        UserScript.OnWeatherset()
        UserScript.Mission_InitWeatherGfxSets()
    end
end

if Mission_InitPlayerColorMapping then
    UserScript.Mission_InitPlayerColorMapping = Mission_InitPlayerColorMapping
    Mission_InitPlayerColorMapping = function ()
        UserScript.Mission_InitPlayerColorMapping()
        UserScript.OnColor()
    end
end
if InitPlayerColorMapping then
    UserScript.InitPlayerColorMapping = InitPlayerColorMapping
    InitPlayerColorMapping = function ()
        UserScript.InitPlayerColorMapping()
        UserScript.OnColor()
    end
end
