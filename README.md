# HearthSimulator

![HearthSimulator](./res.png)

Hearthstone Pack Opening Simulator Plugin Based on BepInEx.

### How to Use

`F7` Check plugin status. And it is recommended to use [BepInExConfigManager.Mono](https://github.com/sinai-dev/BepInExConfigManager/releases) to manage settings.

#### Windows

1. Download the latest version of [BepInEx_x86](https://github.com/BepInEx/BepInEx/releases) and extract it to  `Hearthstone\` 
2. Download original [Mono](https://unity.bepinex.dev/corlibs/2019.4.37.zip) and [Unity](https://unity.bepinex.dev/libraries/2019.4.37.zip) libraries and unpack to `Hearthstone\BepInEx\unstripped_corlib\`. ( PS. Mono and Unity version must same as Hearthstone ) 
3. Edit the `doorstop_config.ini` file replacing the line `dllSearchPathOverride=`with `dllSearchPathOverride=BepInEx\unstripped_corlib` 
4. Download the HearthSimulator [Releases](https://github.com/Pik-4/HearthSimulator/releases) and unzip to  `Hearthstone\BepInEx\plugins` 

#### Mac

1. Download the latest version of [BepInEx_unix](https://github.com/BepInEx/BepInEx/releases) and extract it to  `Hearthstone\` 

2. Download original [Mono](https://unity.bepinex.dev/corlibs/2019.4.37.zip) and [Unity](https://unity.bepinex.dev/libraries/2019.4.37.zip) libraries and unpack to `Hearthstone\BepInEx\unstripped_corlib\`. ( PS. Mono and Unity version must same as Hearthstone ) 

3. Edit the `run_bepinex.sh` file replacing the line `export DOORSTOP_CORLIB_OVERRIDE_PATH=""`with `DOORSTOP_CORLIB_OVERRIDE_PATH="$BASEDIR/BepInEx/unstripped_corlib"` 

4. Edit the file `run_bepinex.sh` replacing the line `executable_name=""` with `executable_name="Hearthstone.app"` 

5. Run command in console `chmod u+x run_bepinex.sh`

6. Get the [token](https://eu.battle.net/login/en-us/?app=wtcg) here and copy after `http://localhost:0/?ST=` and before `&accountId=`

   ```
   # Some verify url
   https://www.battlenet.com.cn/login/zh-cn/?app=wtcg
   https://us.battle.net/login/en/?app=wtcg
   https://tw.battle.net/login/zh/?app=wtcg
   https://kr.battle.net/login/zh/?app=wtcg
   ...
   ```

7. Create a `client.config` file with the following content, instead of `token` - insert the token obtained in the previous step. E.g

   ```
   [Config]
   Version = 3
   [Aurora]
   VerifyWebCredentials = "token"
   ClientCheck = 0
   Env.Override = 1
   Env = eu.actual.battle.net
   ```

8. Download the HearthSimulator [Releases](https://github.com/Pik-4/HearthSimulator/releases) and unzip to  `Hearthstone\BepInEx\plugins` 

Now the game needs to be launched only through `./run_bepinex.sh`

If the token becomes obsolete and the game stops opening, then you just need to update it in the `client.config`.

