// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

using WDG.Business;
using WDG.Models;
using WDG.Utility;
using WDG.Utility.Api;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using WDG.DTO;

namespace WDG.Wallet.Cli
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            bool isTestNet = false;
            if (args.Length>0 && args[0].ToLower() == "-testnet")
            {
                isTestNet = true;
                //删除指定的参数
                List<string> list = args.ToList();
                list.RemoveAt(0);
                args = list.ToArray();
            }
            WalletNetwork.SetNetWork(isTestNet);
            Run(args);
        }

        static void Run(string[] args)
        {
            CommandLineApplication app = new CommandLineApplication(false);
            app.HelpOption("-?|-h|--help");
            app.OnExecute(() =>
            {
                app.ShowHelp();
                return;
            });
            try
            {
                /*
                //先同步区块
                if (await Tools.SyncBlockTool.IsNeedSyncBlock())
                {
                    Console.WriteLine("sync block data, please wait");
                    ProgressBar progressBar = new ProgressBar(Console.CursorLeft, Console.CursorTop, 50, ProgressBarType.Character);
                    await Tools.SyncBlockTool.SyncBlock(progressBar.Dispaly);
                    Console.WriteLine();
                    Console.WriteLine("block data sync successfully");
                }
                */
                #region account command

                app.Command("account", command =>
                {
                    command.Description = "account operation";
                    command.HelpOption("-?|-h|--help");

                    CommandOption tOption = command.Option("-t", "get all account info according to tag", CommandOptionType.NoValue);
                    CommandOption aOption = command.Option("-a", "get single account info according to address", CommandOptionType.NoValue);
                    CommandOption nOption = command.Option("-n", "generate a new account", CommandOptionType.NoValue);
                    CommandOption dOption = command.Option("-d", "get default account", CommandOptionType.NoValue);
                    CommandOption sOption = command.Option("-s", "set default account", CommandOptionType.NoValue);
                    CommandOption vOption = command.Option("-v", "validate address", CommandOptionType.NoValue);
                    CommandOption cOption = command.Option("-c", "set account tag", CommandOptionType.NoValue);

                    CommandArgument argument = command.Argument("address or tag", "execute some option need params", multipleValues: true);
                    command.OnExecute(async () =>
                    {
                        //在这里使用上面各种 Argument 和 Option 的 Value 或 Values 属性拿值。
                        if (args != null && args.Length > 1)
                        {
                            switch ((args[args.Length - 1]).ToLower())
                            {
                                #region get all account info according to tag

                                case "-t":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values != null && argument.Values.Count == 1)
                                    {
                                        ApiResponse response = await AccountsApi.GetAddressesByTag(argument.Values[0]);
                                        if (!response.HasError)
                                        {
                                            JToken result = response.Result;
                                            command.Out.WriteLine(result);
                                            return;
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(response.Error.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("get all account info according to tag need one paramter");
                                        return;
                                    }

                                #endregion

                                #region get single account info according to address

                                case "-a":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values != null && argument.Values.Count == 1)
                                    {
                                        ApiResponse blockChainResponse = await BlockChainEngineApi.GetBlockChainStatus();
                                        if (!blockChainResponse.HasError)
                                        {
                                            BlockChainStatus blockChainStatus = blockChainResponse.GetResult<BlockChainStatus>();
                                            //验证address
                                            if (AddressTools.AddressVerfy(blockChainStatus.ChainNetwork, argument.Values[0]))
                                            {
                                                ApiResponse response = await AccountsApi.GetAccountByAddress(argument.Values[0]);
                                                if (!response.HasError)
                                                {
                                                    JToken result = response.Result;
                                                    command.Out.WriteLine(result);
                                                    return;
                                                }
                                                else
                                                {
                                                    command.Out.WriteLine(response.Error.Message);
                                                    return;
                                                }
                                            }
                                            else
                                            {
                                                command.Out.WriteLine("address is invalid");
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(blockChainResponse.Error.Message);
                                            return;
                                        }

                                    }
                                    else
                                    {
                                        command.Out.WriteLine("get single account info according to address need one paramter");
                                        return;
                                    }

                                #endregion

                                #region generate a new account

                                case "-n":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values != null && argument.Values.Count == 1)
                                    {
                                        bool isEncrypt = await IsEncrypt();
                                        if (isEncrypt)
                                        {
                                            string password = Prompt.GetPassword("please input your password to unlock:");
                                            if (!IsPasswordValid(password))
                                            {
                                                command.Out.WriteLine("password is invalid");
                                                return;
                                            }
                                            else
                                            {
                                                ApiResponse walletResponse = await WalletManagementApi.WalletPassphrase(password);
                                                if (walletResponse.HasError)
                                                {
                                                    command.Out.WriteLine("password is incorrect");
                                                    return;
                                                }
                                            }
                                        }
                                        ApiResponse response = await AccountsApi.GetNewAddress(argument.Values[0]);
                                        if (!response.HasError)
                                        {
                                            JToken result = response.Result;
                                            command.Out.WriteLine(result);
                                            return;
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(response.Error.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("generate a new account need one paramter");
                                        return;
                                    }

                                #endregion

                                #region get default account

                                case "-d":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 0)
                                    {
                                        ApiResponse response = await AccountsApi.GetDefaultAccount();
                                        if (!response.HasError)
                                        {
                                            JToken result = response.Result;
                                            command.Out.WriteLine(result);
                                            return;
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(response.Error.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("get all account info according to tag need none paramter");
                                        return;
                                    }

                                #endregion

                                #region set default account

                                case "-s":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values != null && argument.Values.Count == 1)
                                    {
                                        ApiResponse blockChainResponse = await BlockChainEngineApi.GetBlockChainStatus();
                                        if (!blockChainResponse.HasError)
                                        {
                                            BlockChainStatus blockChainStatus = blockChainResponse.GetResult<BlockChainStatus>();
                                            //验证address
                                            if (AddressTools.AddressVerfy(blockChainStatus.ChainNetwork, argument.Values[0]))
                                            {
                                                ApiResponse response = await AccountsApi.SetDefaultAccount(argument.Values[0]);
                                                if (!response.HasError)
                                                {
                                                    command.Out.WriteLine("executes successfully");
                                                    return;
                                                }
                                                else
                                                {
                                                    command.Out.WriteLine(response.Error.Message);
                                                    return;
                                                }
                                            }
                                            else
                                            {
                                                command.Out.WriteLine("address is invalid");
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(blockChainResponse.Error.Message);
                                            return;
                                        }

                                    }
                                    else
                                    {
                                        command.Out.WriteLine("set default account need one paramter");
                                        return;
                                    }

                                #endregion

                                #region validate address

                                case "-v":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values != null && argument.Values.Count == 1)
                                    {
                                        ApiResponse blockChainResponse = await BlockChainEngineApi.GetBlockChainStatus();
                                        if (!blockChainResponse.HasError)
                                        {
                                            BlockChainStatus blockChainStatus = blockChainResponse.GetResult<BlockChainStatus>();
                                            //验证address
                                            if (AddressTools.AddressVerfy(blockChainStatus.ChainNetwork, argument.Values[0]))
                                            {
                                                ApiResponse response = await AccountsApi.ValidateAddress(argument.Values[0]);
                                                if (!response.HasError)
                                                {
                                                    JToken result = response.Result;
                                                    command.Out.WriteLine(result);
                                                    return;
                                                }
                                                else
                                                {
                                                    command.Out.WriteLine(response.Error.Message);
                                                    return;
                                                }
                                            }
                                            else
                                            {
                                                command.Out.WriteLine("invalid address");
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(blockChainResponse.Error.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("validate address need one paramter");
                                        return;
                                    }

                                #endregion

                                #region set account tag

                                case "-c":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values != null && argument.Values.Count == 2)
                                    {
                                        ApiResponse blockChainResponse = await BlockChainEngineApi.GetBlockChainStatus();
                                        if (!blockChainResponse.HasError)
                                        {
                                            BlockChainStatus blockChainStatus = blockChainResponse.GetResult<BlockChainStatus>();
                                            //验证address
                                            if (AddressTools.AddressVerfy(blockChainStatus.ChainNetwork, argument.Values[0]))
                                            {
                                                ApiResponse response = await AccountsApi.SetAccountTag(argument.Values[0], argument.Values[1]);
                                                if (!response.HasError)
                                                {
                                                    command.Out.WriteLine("executes successfully");
                                                    return;
                                                }
                                                else
                                                {
                                                    command.Out.WriteLine(response.Error.Message);
                                                    return;
                                                }
                                            }
                                            else
                                            {
                                                command.Out.WriteLine("Invalid address");
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(blockChainResponse.Error.Message);
                                            return;
                                        }

                                    }
                                    else
                                    {
                                        command.Out.WriteLine("set account tag need two paramters");
                                        return;
                                    }

                                #endregion

                                default:
                                    command.ShowHelp();
                                    return;
                            }
                        }
                        else
                        {
                            command.ShowHelp();
                            return;
                        }
                    });

                });

                #endregion

                #region address command

                app.Command("address", command =>
                {
                    command.Description = "address operation";
                    command.HelpOption("-?|-h|--help");

                    CommandOption aOption = command.Option("-a", "add new addressbook item", CommandOptionType.NoValue);
                    CommandOption gOption = command.Option("-g", "get addressbook info", CommandOptionType.NoValue);
                    CommandOption bOption = command.Option("-b", "get addressbook item by address", CommandOptionType.NoValue);
                    CommandOption tOption = command.Option("-t", "get addressbook by tag", CommandOptionType.NoValue);
                    CommandOption dOption = command.Option("-d", "delete addressbook items by ids", CommandOptionType.NoValue);

                    CommandArgument argument = command.Argument("address, tag, ids or label", "execute some option need params", multipleValues: true);
                    command.OnExecute(async () =>
                    {
                        //在这里使用上面各种 Argument 和 Option 的 Value 或 Values 属性拿值。
                        if (args != null && args.Length > 1)
                        {
                            switch ((args[args.Length - 1]).ToLower())
                            {
                                #region add new addressbook item

                                case "-a":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values != null && argument.Values.Count == 2)
                                    {
                                        ApiResponse blockChainResponse = await BlockChainEngineApi.GetBlockChainStatus();
                                        if (!blockChainResponse.HasError)
                                        {
                                            BlockChainStatus blockChainStatus = blockChainResponse.GetResult<BlockChainStatus>();
                                            //验证address
                                            if (AddressTools.AddressVerfy(blockChainStatus.ChainNetwork, argument.Values[0]))
                                            {
                                                ApiResponse response = await AddressBookApi.AddNewAddressBookItem(argument.Values[0], argument.Values[1]);
                                                if (!response.HasError)
                                                {
                                                    command.Out.WriteLine("executes successfully");
                                                    return;
                                                }
                                                else
                                                {
                                                    command.Out.WriteLine(response.Error.Message);
                                                    return;
                                                }
                                            }
                                            else
                                            {
                                                command.Out.WriteLine("invalid address");
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(blockChainResponse.Error.Message);
                                            return;
                                        }

                                    }
                                    else
                                    {
                                        command.Out.WriteLine("add new addressbook item command need two paramters");
                                        return;
                                    }

                                #endregion

                                #region get addressbook info

                                case "-g":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 0)
                                    {
                                        ApiResponse response = await AddressBookApi.GetAddressBook();
                                        if (!response.HasError)
                                        {

                                            if (!response.HasError)
                                            {
                                                JToken result = response.Result;
                                                command.Out.WriteLine(result);
                                                return;
                                            }
                                            else
                                            {
                                                command.Out.WriteLine(response.Error.Message);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(response.Error.Message);
                                            return;
                                        }

                                    }
                                    else
                                    {
                                        command.Out.WriteLine("get addressbook info command need none paramter");
                                        return;
                                    }

                                #endregion

                                #region get addressbook item by address

                                case "-b":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values != null && argument.Values.Count == 1)
                                    {
                                        ApiResponse blockChainResponse = await BlockChainEngineApi.GetBlockChainStatus();
                                        if (!blockChainResponse.HasError)
                                        {
                                            BlockChainStatus blockChainStatus = blockChainResponse.GetResult<BlockChainStatus>();
                                            //验证address
                                            if (AddressTools.AddressVerfy(blockChainStatus.ChainNetwork, argument.Values[0]))
                                            {
                                                ApiResponse response = await AddressBookApi.GetAddressBookItemByAddress(argument.Values[0]);
                                                if (!response.HasError)
                                                {
                                                    JToken result = response.Result;
                                                    command.Out.WriteLine(result);
                                                    return;
                                                }
                                                else
                                                {
                                                    command.Out.WriteLine(response.Error.Message);
                                                    return;
                                                }
                                            }
                                            else
                                            {
                                                command.Out.WriteLine("invalid address");
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(blockChainResponse.Error.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("get addressbook item by address command need one paramter");
                                        return;
                                    }

                                #endregion

                                #region get addressbook by tag

                                case "-t":
                                    //需要解锁
                                    if (argument != null && argument.Values != null && argument.Values.Count == 1)
                                    {
                                        bool isEncrypt = await IsEncrypt();
                                        if (isEncrypt)
                                        {
                                            string password = Prompt.GetPassword("please input your password to unlock:");
                                            if (!IsPasswordValid(password))
                                            {
                                                command.Out.WriteLine("password is invalid");
                                                return;
                                            }
                                            else
                                            {
                                                ApiResponse walletResponse = await WalletManagementApi.WalletPassphrase(password);
                                                if (walletResponse.HasError)
                                                {
                                                    command.Out.WriteLine("password is incorrect");
                                                    return;
                                                }
                                                else
                                                {
                                                    ApiResponse response = await AddressBookApi.GetAddressBookByTag(argument.Values[0]);
                                                    if (!response.HasError)
                                                    {
                                                        JToken result = response.Result;
                                                        command.Out.WriteLine(result);
                                                        ApiResponse lockResponse = await WalletManagementApi.WalletLock();
                                                        if (lockResponse.HasError)
                                                        {
                                                            command.Out.WriteLine("lock failure");
                                                        }
                                                        return;
                                                    }
                                                    else
                                                    {
                                                        command.Out.WriteLine(response.Error.Message);
                                                        return;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ApiResponse response = await AddressBookApi.GetAddressBookByTag(argument.Values[0]);
                                            if (!response.HasError)
                                            {
                                                JToken result = response.Result;
                                                command.Out.WriteLine(result);
                                                return;
                                            }
                                            else
                                            {
                                                command.Out.WriteLine(response.Error.Message);
                                                return;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("get addressbook by tag command need one paramter");
                                        return;
                                    }

                                #endregion

                                #region delete addressbook items by ids

                                case "-d":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values != null && argument.Values.Count == 1)
                                    {
                                        long[] idsLongArray = null;
                                        try
                                        {
                                            string[] idsStringArray = argument.Values[0].Split(',');
                                            idsLongArray = Array.ConvertAll(idsStringArray, s => long.Parse(s));
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.Singleton.Error(ex.ToString());
                                            command.Out.WriteLine("parameter is invalid");
                                            return;
                                        }
                                        ApiResponse response = await AddressBookApi.DeleteAddressBookByIds(idsLongArray);
                                        if (!response.HasError)
                                        {
                                            command.Out.WriteLine("executes successfully");
                                            return;
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(response.Error.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("delete addressbook items by ids command need one paramter");
                                        return;
                                    }

                                #endregion

                                default:
                                    command.ShowHelp();
                                    return;
                            }
                        }
                        else
                        {
                            command.ShowHelp();
                            return;
                        }
                    });

                });

                #endregion

                #region block command

                app.Command("block", command =>
                {
                    command.Description = "block chain operation";
                    command.HelpOption("-?|-h|--help");

                    CommandOption uOption = command.Option("-u", "get block chain status", CommandOptionType.NoValue);
                    CommandOption nOption = command.Option("-n", "stop block engine", CommandOptionType.NoValue);
                    CommandOption cOption = command.Option("-c", "get block count", CommandOptionType.NoValue);
                    CommandOption iOption = command.Option("-i", "get block hash", CommandOptionType.NoValue);
                    CommandOption bOption = command.Option("-b", "get block", CommandOptionType.NoValue);
                    CommandOption eOption = command.Option("-e", "get block header", CommandOptionType.NoValue);
                    CommandOption tOption = command.Option("-t", "get block chain tips", CommandOptionType.NoValue);
                    CommandOption dOption = command.Option("-d", "get block algorithm difficulty", CommandOptionType.NoValue);
                    CommandOption gOption = command.Option("-g", "generate new block", CommandOptionType.NoValue);
                    CommandOption sOption = command.Option("-s", "submit block data", CommandOptionType.NoValue);
                    //CommandOption mOption = command.Option("-m", "sign the message", CommandOptionType.NoValue);

                    CommandArgument argument = command.Argument("arguments", "execute some option need params", multipleValues: true);
                    command.OnExecute(async () =>
                    {
                        //在这里使用上面各种 Argument 和 Option 的 Value 或 Values 属性拿值。
                        if (args != null && args.Length > 1)
                        {
                            switch ((args[args.Length - 1]).ToLower())
                            {
                                #region get block chain status

                                case "-u":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 0)
                                    {
                                        ApiResponse response = await BlockChainEngineApi.GetBlockChainStatus();

                                        if (!response.HasError)
                                        {
                                            JToken result = response.Result;
                                            command.Out.WriteLine(result);
                                            return;
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(response.Error.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("get block chain status command need none paramters");
                                        return;
                                    }

                                #endregion

                                #region stop block engine

                                case "-n":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 0)
                                    {
                                        ApiResponse response = await BlockChainEngineApi.StopEngine();
                                        if (!response.HasError)
                                        {
                                            command.Out.WriteLine("execute successfully");
                                            return;
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(response.Error.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("stop block engine command need none paramter");
                                        return;
                                    }

                                #endregion

                                #region get block count

                                case "-c":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 0)
                                    {
                                        ApiResponse response = await BlockChainEngineApi.GetBlockCount();
                                        if (!response.HasError)
                                        {
                                            JToken result = response.Result;
                                            command.Out.WriteLine(result);
                                            return;
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(response.Error.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("get block count command need none paramters");
                                        return;
                                    }

                                #endregion

                                #region get block hash

                                case "-i":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values != null && argument.Values.Count == 1)
                                    {
                                        int height = 0;
                                        if (int.TryParse(argument.Values[0], out height))
                                        {
                                            ApiResponse response = await BlockChainEngineApi.GetBlockHash(height);
                                            if (!response.HasError)
                                            {
                                                JToken result = response.Result;
                                                command.Out.WriteLine(result);
                                                return;
                                            }
                                            else
                                            {
                                                command.Out.WriteLine(response.Error.Message);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            command.Out.WriteLine("invalid parameter");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("get block hash command need one paramter");
                                        return;
                                    }

                                #endregion

                                #region get block info

                                case "-b":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values != null && argument.Values.Count == 2)
                                    {
                                        if (argument.Values[1] == "1" || argument.Values[1] == "0")
                                        {
                                            ApiResponse response = await BlockChainEngineApi.GetBlock(argument.Values[0], int.Parse(argument.Values[1]));
                                            if (!response.HasError)
                                            {
                                                JToken result = response.Result;
                                                command.Out.WriteLine(result);
                                                return;
                                            }
                                            else
                                            {
                                                command.Out.WriteLine(response.Error.Message);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            command.Out.WriteLine("invalid parameter");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("get block info command need two paramter");
                                        return;
                                    }

                                #endregion

                                #region get block header

                                case "-e":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values != null && argument.Values.Count == 2)
                                    {
                                        if (argument.Values[1] == "1" || argument.Values[1] == "0")
                                        {
                                            ApiResponse response = await BlockChainEngineApi.GetBlockHeader(argument.Values[0], int.Parse(argument.Values[1]));
                                            if (!response.HasError)
                                            {
                                                JToken result = response.Result;
                                                command.Out.WriteLine(result);
                                                return;
                                            }
                                            else
                                            {
                                                command.Out.WriteLine(response.Error.Message);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            command.Out.WriteLine("invalid parameter");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("get block header command need two paramter");
                                        return;
                                    }

                                #endregion

                                #region get block chain tips

                                case "-t":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 0)
                                    {
                                        ApiResponse response = await BlockChainEngineApi.GetChainTips();
                                        if (!response.HasError)
                                        {
                                            JToken result = response.Result;
                                            command.Out.WriteLine(result);
                                            return;
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(response.Error.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("get block chain tips command need none paramters");
                                        return;
                                    }

                                #endregion

                                #region get block algorithm difficulty

                                case "-d":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 0)
                                    {
                                        ApiResponse response = await BlockChainEngineApi.GetDifficulty();
                                        if (!response.HasError)
                                        {
                                            JToken result = response.Result;
                                            command.Out.WriteLine(result);
                                            return;
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(response.Error.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("get block algorithm difficulty command need none paramters");
                                        return;
                                    }

                                #endregion

                                #region generate new block

                                case "-g":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values != null && argument.Values.Count == 3)
                                    {
                                        ApiResponse blockChainResponse = await BlockChainEngineApi.GetBlockChainStatus();
                                        if (!blockChainResponse.HasError)
                                        {
                                            BlockChainStatus blockChainStatus = blockChainResponse.GetResult<BlockChainStatus>();
                                            //验证address
                                            if (AddressTools.AddressVerfy(blockChainStatus.ChainNetwork, argument.Values[1]))
                                            {
                                                if (argument.Values[2] == "1" || argument.Values[2] == "0")
                                                {
                                                    ApiResponse response = await BlockChainEngineApi.GenerateNewBlock(argument.Values[0], argument.Values[1], int.Parse(argument.Values[2]));
                                                    if (!response.HasError)
                                                    {
                                                        JToken result = response.Result;
                                                        command.Out.WriteLine(result);
                                                        return;
                                                    }
                                                    else
                                                    {
                                                        command.Out.WriteLine(response.Error.Message);
                                                        return;
                                                    }
                                                }
                                                else
                                                {
                                                    command.Out.WriteLine("invalid parameter");
                                                    return;
                                                }
                                            }
                                            else
                                            {
                                                command.Out.WriteLine("invalid address");
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(blockChainResponse.Error.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("generate new block command need three paramter");
                                        return;
                                    }

                                #endregion

                                #region submit block data

                                case "-s":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values != null && argument.Values.Count == 1)
                                    {
                                        ApiResponse response = await BlockChainEngineApi.SubmitBlock(argument.Values[0]);
                                        if (!response.HasError)
                                        {
                                            command.Out.WriteLine("executes successfully");
                                            return;
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(response.Error.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("submit block data command need one paramter");
                                        return;
                                    }

                                #endregion

                                default:
                                    command.ShowHelp();
                                    return;
                            }
                        }
                        else
                        {
                            command.ShowHelp();
                            return;
                        }
                    });

                });

                #endregion

                #region memory command

                app.Command("memory", command =>
                {
                    command.Description = "memory pool operation";
                    command.HelpOption("-?|-h|--help");

                    CommandOption aOption = command.Option("-a", "get all tx in memory pool", CommandOptionType.NoValue);
                    CommandOption pOption = command.Option("-p", "get payment info in memory pool", CommandOptionType.NoValue);

                    CommandArgument argument = command.Argument("arguments", "execute some option need params", multipleValues: true);
                    command.OnExecute(async () =>
                    {
                        //在这里使用上面各种 Argument 和 Option 的 Value 或 Values 属性拿值。
                        if (args != null && args.Length > 1)
                        {
                            switch ((args[args.Length - 1]).ToLower())
                            {
                                #region get all tx in memory pool

                                case "-a":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 0)
                                    {
                                        ApiResponse response = await MemoryPoolApi.GetAllTxInMemPool();

                                        if (!response.HasError)
                                        {
                                            JToken result = response.Result;
                                            command.Out.WriteLine(result);
                                            return;
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(response.Error.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("get all tx in memory pool command need none paramters");
                                        return;
                                    }

                                #endregion

                                #region get payment info in memory pool

                                case "-p":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 1)
                                    {
                                        ApiResponse response = await MemoryPoolApi.GetPaymentInfoInMemPool(argument.Values[0]);
                                        if (!response.HasError)
                                        {
                                            JToken result = response.Result;
                                            command.Out.WriteLine(result);
                                            return;
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(response.Error.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("get payment info in memory pool command need one paramter");
                                        return;
                                    }

                                #endregion

                                default:
                                    command.ShowHelp();
                                    return;
                            }
                        }
                        else
                        {
                            command.ShowHelp();
                            return;
                        }
                    });

                });

                #endregion

                #region net command

                app.Command("net", command =>
                {
                    command.Description = "network operation";
                    command.HelpOption("-?|-h|--help");

                    CommandOption iOption = command.Option("-i", "get network info", CommandOptionType.NoValue);
                    CommandOption tOption = command.Option("-t", "get net totals", CommandOptionType.NoValue);
                    CommandOption sOption = command.Option("-s", "get connection count", CommandOptionType.NoValue);
                    CommandOption pOption = command.Option("-p", "get peer info", CommandOptionType.NoValue);
                    CommandOption aOption = command.Option("-a", "add node", CommandOptionType.NoValue);
                    CommandOption nOption = command.Option("-n", "get added node info", CommandOptionType.NoValue);
                    CommandOption dOption = command.Option("-d", "disconnect node", CommandOptionType.NoValue);
                    CommandOption bOption = command.Option("-b", "set ban", CommandOptionType.NoValue);
                    CommandOption lOption = command.Option("-l", "list banned", CommandOptionType.NoValue);
                    CommandOption cOption = command.Option("-c", "clear banned", CommandOptionType.NoValue);
                    CommandOption vOption = command.Option("-v", "set network active", CommandOptionType.NoValue);
                    CommandOption oOption = command.Option("-o", "get block chain info", CommandOptionType.NoValue);

                    CommandArgument argument = command.Argument("arguments", "execute some option need params", multipleValues: true);
                    command.OnExecute(async () =>
                    {
                        //在这里使用上面各种 Argument 和 Option 的 Value 或 Values 属性拿值。
                        if (args != null && args.Length > 1)
                        {
                            switch ((args[args.Length - 1]).ToLower())
                            {
                                #region get network info

                                case "-i":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 0)
                                    {
                                        ApiResponse response = await NetworkApi.GetNetworkInfo();

                                        if (!response.HasError)
                                        {
                                            JToken result = response.Result;
                                            command.Out.WriteLine(result);
                                            return;
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(response.Error.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("get network info command need none paramters");
                                        return;
                                    }

                                #endregion

                                #region get net totals

                                case "-t":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 0)
                                    {
                                        ApiResponse response = await NetworkApi.GetNetTotals();

                                        if (!response.HasError)
                                        {
                                            JToken result = response.Result;
                                            command.Out.WriteLine(result);
                                            return;
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(response.Error.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("get net totals command need none paramters");
                                        return;
                                    }

                                #endregion

                                #region get connection count

                                case "-s":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 0)
                                    {
                                        ApiResponse response = await NetworkApi.GetConnectionCount();

                                        if (!response.HasError)
                                        {
                                            JToken result = response.Result;
                                            command.Out.WriteLine(result);
                                            return;
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(response.Error.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("get connection count command need none paramters");
                                        return;
                                    }

                                #endregion

                                #region get peer info

                                case "-p":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 0)
                                    {
                                        ApiResponse response = await NetworkApi.GetPeerInfo();

                                        if (!response.HasError)
                                        {
                                            JToken result = response.Result;
                                            command.Out.WriteLine(result);
                                            return;
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(response.Error.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("get peer info command need none paramters");
                                        return;
                                    }

                                #endregion

                                #region add node

                                case "-a":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 1)
                                    {
                                        Regex reg = new Regex(@"((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?[1-9])))(\:\d)*)");
                                        if (reg.IsMatch(argument.Values[0]))
                                        {
                                            ApiResponse response = await NetworkApi.AddNode(argument.Values[0]);

                                            if (!response.HasError)
                                            {
                                                command.Out.WriteLine("execute successfully");
                                                return;
                                            }
                                            else
                                            {
                                                command.Out.WriteLine(response.Error.Message);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            command.Out.WriteLine("invalid address with port");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("get net totals command need one paramter");
                                        return;
                                    }

                                #endregion

                                #region get added node info

                                case "-n":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 1)
                                    {
                                        Regex reg = new Regex(@"((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?[1-9])))(\:\d)*)");
                                        if (reg.IsMatch(argument.Values[0]))
                                        {
                                            ApiResponse response = await NetworkApi.GetAddedNodeInfo(argument.Values[0]);
                                            if (!response.HasError)
                                            {
                                                JToken result = response.Result;
                                                command.Out.WriteLine(result);
                                                return;
                                            }
                                            else
                                            {
                                                command.Out.WriteLine(response.Error.Message);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            command.Out.WriteLine("invalid address with port");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("get added node info command need one paramter");
                                        return;
                                    }

                                #endregion

                                #region disconnect node

                                case "-d":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 1)
                                    {
                                        Regex reg = new Regex(@"((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?[1-9])))(\:\d)*)");
                                        if (reg.IsMatch(argument.Values[0]))
                                        {
                                            ApiResponse response = await NetworkApi.DisconnectNode(argument.Values[0]);

                                            if (!response.HasError)
                                            {
                                                bool result = response.GetResult<bool>();
                                                if (result)
                                                {
                                                    command.Out.WriteLine("node disconnect");
                                                }
                                                else
                                                {
                                                    command.Out.WriteLine("node disconnect");
                                                }
                                                return;
                                            }
                                            else
                                            {
                                                command.Out.WriteLine(response.Error.Message);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            command.Out.WriteLine("invalid address with port");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("disconnect node command need one paramter");
                                        return;
                                    }

                                #endregion

                                #region set ban

                                case "-b":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 2)
                                    {
                                        Regex reg = new Regex(@"((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?[1-9])))(\:\d)*)");
                                        if (reg.IsMatch(argument.Values[0]))
                                        {
                                            ApiResponse response = await NetworkApi.SetBan(argument.Values[0], argument.Values[1]);

                                            if (!response.HasError)
                                            {
                                                command.Out.WriteLine("execute successfully");
                                                return;
                                            }
                                            else
                                            {
                                                command.Out.WriteLine(response.Error.Message);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            command.Out.WriteLine("invalid address with port");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("set ban command need two paramters");
                                        return;
                                    }

                                #endregion

                                #region list banned

                                case "-l":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 0)
                                    {
                                        ApiResponse response = await NetworkApi.ListBanned();

                                        if (!response.HasError)
                                        {
                                            JToken result = response.Result;
                                            command.Out.WriteLine(result);
                                            return;
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(response.Error.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("list banned command need none paramters");
                                        return;
                                    }

                                #endregion

                                #region clear banned

                                case "-c":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 0)
                                    {
                                        ApiResponse response = await NetworkApi.ClearBanned();

                                        if (!response.HasError)
                                        {
                                            command.Out.WriteLine("execute successfully");
                                            return;
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(response.Error.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("clear banned command need none paramters");
                                        return;
                                    }

                                #endregion

                                #region set network active

                                case "-v":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 1)
                                    {
                                        if (argument.Values[0] == "true" || argument.Values[0] == "false")
                                        {
                                            ApiResponse response = await NetworkApi.SetNetworkActive(bool.Parse(argument.Values[0]));

                                            if (!response.HasError)
                                            {
                                                command.Out.WriteLine("execute successfully");
                                                return;
                                            }
                                            else
                                            {
                                                command.Out.WriteLine(response.Error.Message);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            command.Out.WriteLine("invalid parameter");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("set network active command need one paramter");
                                        return;
                                    }

                                #endregion

                                #region get block chain info

                                case "-o":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 0)
                                    {
                                        ApiResponse response = await NetworkApi.GetBlockChainInfo();

                                        if (!response.HasError)
                                        {
                                            JToken result = response.Result;
                                            command.Out.WriteLine(result);
                                            return;
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(response.Error.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("get block chain info command need none paramters");
                                        return;
                                    }

                                #endregion

                                default:
                                    command.ShowHelp();
                                    return;
                            }
                        }
                        else
                        {
                            command.ShowHelp();
                            return;
                        }
                    });

                });

                #endregion

                #region pay command

                app.Command("pay", command =>
                {
                    command.Description = "pay request operation";
                    command.HelpOption("-?|-h|--help");

                    CommandOption cOption = command.Option("-c", "create new payment request", CommandOptionType.NoValue);
                    CommandOption aOption = command.Option("-a", "get all payment requests", CommandOptionType.NoValue);
                    CommandOption dOption = command.Option("-d", "delete payment requests by ids", CommandOptionType.NoValue);

                    CommandArgument argument = command.Argument("arguments", "execute some option need params", multipleValues: true);
                    command.OnExecute(async () =>
                    {
                        //在这里使用上面各种 Argument 和 Option 的 Value 或 Values 属性拿值。
                        if (args != null && args.Length > 1)
                        {
                            switch ((args[args.Length - 1]).ToLower())
                            {
                                #region create new payment request

                                case "-c":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 3)
                                    {
                                        if (long.TryParse(argument.Values[1], out long amount))
                                        {
                                            //判断钱包是否加密
                                            bool isEncrypt = await IsEncrypt();
                                            if (isEncrypt)
                                            {
                                                string password = Prompt.GetPassword("please input your password to unlock:");
                                                if (!IsPasswordValid(password))
                                                {
                                                    command.Out.WriteLine("password is invalid");
                                                    return;
                                                }
                                                else
                                                {
                                                    ApiResponse walletResponse = await WalletManagementApi.WalletPassphrase(password);
                                                    if (walletResponse.HasError)
                                                    {
                                                        command.Out.WriteLine("password is incorrect");
                                                        return;
                                                    }
                                                }
                                            }
                                            ApiResponse response = await PaymentRequestApi.CreateNewPaymentRequest(argument.Values[0], amount, argument.Values[2]);
                                            if (!response.HasError)
                                            {
                                                JToken result = response.Result;
                                                command.Out.WriteLine(result);
                                                return;
                                            }
                                            else
                                            {
                                                command.Out.WriteLine(response.Error.Message);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            command.Out.WriteLine("invalid parameter");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("create new payment request command need three paramters");
                                        return;
                                    }

                                #endregion

                                #region get all payment requests

                                case "-a":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 0)
                                    {
                                        ApiResponse response = await PaymentRequestApi.GetAllPaymentRequests();
                                        if (!response.HasError)
                                        {
                                            JToken result = response.Result;
                                            command.Out.WriteLine(result);
                                            return;
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(response.Error.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("get all payment requests command need none paramters");
                                        return;
                                    }

                                #endregion

                                #region delete payment requests by ids

                                case "-d":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 1)
                                    {
                                        long[] longArray = null;
                                        try
                                        {
                                            string[] stringArray = argument.Values[0].Split(',');
                                            longArray = Array.ConvertAll(stringArray, s => long.Parse(s));
                                        }
                                        catch
                                        {
                                            command.Out.WriteLine("invalid parameter");
                                            return;
                                        }
                                        ApiResponse response = await PaymentRequestApi.DeletePaymentRequestsByIds(longArray);
                                        if (!response.HasError)
                                        {
                                            command.Out.WriteLine("execute successfully");
                                            return;
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(response.Error.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("delete payment requests by ids command need one paramter");
                                        return;
                                    }

                                #endregion

                                default:
                                    command.ShowHelp();
                                    return;
                            }
                        }
                        else
                        {
                            command.ShowHelp();
                            return;
                        }
                    });

                });

                #endregion

                #region trans command

                app.Command("trans", command =>
                {
                    command.Description = "transaction operation";
                    command.HelpOption("-?|-h|--help");

                    CommandOption mOption = command.Option("-m", "transaction to many address", CommandOptionType.NoValue);
                    CommandOption aOption = command.Option("-a", "transaction to give address", CommandOptionType.NoValue);
                    CommandOption fOption = command.Option("-f", "set transaction fee", CommandOptionType.NoValue);
                    CommandOption cOption = command.Option("-c", "set transaction confirmations", CommandOptionType.NoValue);
                    CommandOption sOption = command.Option("-s", "get transaction settings", CommandOptionType.NoValue);
                    CommandOption eOption = command.Option("-e", "estimate tx fee for send to address", CommandOptionType.NoValue);
                    CommandOption tOption = command.Option("-t", "estimate tx fee for send many address", CommandOptionType.NoValue);
                    CommandOption lOption = command.Option("-l", "list transactions", CommandOptionType.NoValue);
                    CommandOption rOption = command.Option("-r", "send raw transaction", CommandOptionType.NoValue);
                    CommandOption iOption = command.Option("-i", "estimate raw transactions", CommandOptionType.NoValue);
                    CommandOption nOption = command.Option("-n", "List since block", CommandOptionType.NoValue);
                    CommandOption dOption = command.Option("-d", "get transaction", CommandOptionType.NoValue);

                    CommandArgument argument = command.Argument("arguments", "execute some option need params", multipleValues: true);
                    command.OnExecute(async () =>
                    {
                        //在这里使用上面各种 Argument 和 Option 的 Value 或 Values 属性拿值。
                        if (args != null && args.Length > 1)
                        {
                            switch ((args[args.Length - 1]).ToLower())
                            {
                                #region transaction to many address

                                case "-m":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 3)
                                    {
                                        //这个需要传入一个Json数组参数，最复杂的参数
                                        SendManyModel[] model = null;
                                        string[] subtractFeeFromAmount = null;
                                        try
                                        {
                                            model = Newtonsoft.Json.JsonConvert.DeserializeObject<SendManyModel[]>(argument.Values[1]);
                                            if (!string.IsNullOrEmpty(argument.Values[2]))
                                            {
                                                subtractFeeFromAmount = argument.Values[2].Split(',');
                                            }
                                            //循环判断地址是否合法
                                            ApiResponse blockChainResponse = await BlockChainEngineApi.GetBlockChainStatus();
                                            if (!blockChainResponse.HasError)
                                            {
                                                BlockChainStatus blockChainStatus = blockChainResponse.GetResult<BlockChainStatus>();
                                                //验证address
                                                for (int i = 0; i < model.Length; i++)
                                                {
                                                    if (!AddressTools.AddressVerfy(blockChainStatus.ChainNetwork, model[i].Address))
                                                    {
                                                        command.Out.WriteLine("invalid address format");
                                                        return;
                                                    }
                                                }
                                                for (int j = 0; j < subtractFeeFromAmount.Length; j++)
                                                {
                                                    if (!AddressTools.AddressVerfy(blockChainStatus.ChainNetwork, subtractFeeFromAmount[j]))
                                                    {
                                                        command.Out.WriteLine("invalid address format");
                                                        return;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                command.Out.WriteLine(blockChainResponse.Error.Message);
                                                return;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            command.Out.WriteLine(ex.ToString());
                                            return;
                                        }
                                        bool isEncrypt = await IsEncrypt();
                                        if (isEncrypt)
                                        {
                                            string password = Prompt.GetPassword("please input your password to unlock:");
                                            if (!IsPasswordValid(password))
                                            {
                                                command.Out.WriteLine("password is invalid");
                                                return;
                                            }
                                            else
                                            {
                                                ApiResponse walletResponse = await WalletManagementApi.WalletPassphrase(password);
                                                if (walletResponse.HasError)
                                                {
                                                    command.Out.WriteLine("password is incorrect");
                                                    return;
                                                }
                                                bool isConfirm = Prompt.GetYesNo("confirm your transaction, do your want to continue:", true);
                                                if (!isConfirm)
                                                {
                                                    return;
                                                }
                                                ApiResponse response = await TransactionApi.SendMany(argument.Values[0], model, subtractFeeFromAmount);
                                                if (!response.HasError)
                                                {
                                                    command.Out.WriteLine(response.Result);
                                                    ApiResponse lockResponse = await WalletManagementApi.WalletLock();
                                                    if (lockResponse.HasError)
                                                    {
                                                        command.Out.WriteLine("lock failure");
                                                    }
                                                    return;
                                                }
                                                else
                                                {
                                                    command.Out.WriteLine(response.Error.Message);
                                                    return;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            bool isConfirm = Prompt.GetYesNo("confirm your transaction, do your want to continue:", true);
                                            if (!isConfirm)
                                            {
                                                return;
                                            }
                                            ApiResponse response = await TransactionApi.SendMany(argument.Values[0], model, subtractFeeFromAmount);
                                            if (!response.HasError)
                                            {
                                                command.Out.WriteLine(response.Result);
                                                return;
                                            }
                                            else
                                            {
                                                command.Out.WriteLine(response.Error.Message);
                                                return;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("transaction to many address command need three paramters");
                                        return;
                                    }

                                #endregion

                                #region transaction to give address

                                case "-a":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 5)
                                    {
                                        ApiResponse blockChainResponse = await BlockChainEngineApi.GetBlockChainStatus();
                                        if (!blockChainResponse.HasError)
                                        {
                                            BlockChainStatus blockChainStatus = blockChainResponse.GetResult<BlockChainStatus>();
                                            //验证address
                                            try
                                            {
                                                if (AddressTools.AddressVerfy(blockChainStatus.ChainNetwork, argument.Values[0]))
                                                {
                                                    if (argument.Values[4] == "true" || argument.Values[4] == "false")
                                                    {
                                                        bool isEncrypt = await IsEncrypt();
                                                        if (isEncrypt)
                                                        {
                                                            string password = Prompt.GetPassword("please input your password to unlock:");
                                                            if (!IsPasswordValid(password))
                                                            {
                                                                command.Out.WriteLine("password is invalid");
                                                                return;
                                                            }
                                                            else
                                                            {
                                                                ApiResponse walletResponse = await WalletManagementApi.WalletPassphrase(password);
                                                                if (walletResponse.HasError)
                                                                {
                                                                    command.Out.WriteLine("password is incorrect");
                                                                    return;
                                                                }
                                                                bool isConfirm = Prompt.GetYesNo("confirm your transaction, do your want to continue:", true);
                                                                if (!isConfirm)
                                                                {
                                                                    return;
                                                                }
                                                                ApiResponse response = await TransactionApi.SendToAddress(argument.Values[0], long.Parse(argument.Values[1]), argument.Values[2], argument.Values[3], bool.Parse(argument.Values[4]));
                                                                if (!response.HasError)
                                                                {
                                                                    command.Out.WriteLine(response.Result);
                                                                    ApiResponse lockResponse = await WalletManagementApi.WalletLock();
                                                                    if (lockResponse.HasError)
                                                                    {
                                                                        command.Out.WriteLine("lock failure");
                                                                    }
                                                                    return;
                                                                }
                                                                else
                                                                {
                                                                    command.Out.WriteLine(response.Error.Message);
                                                                    return;
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            bool isConfirm = Prompt.GetYesNo("confirm your transaction, do your want to continue:", true);
                                                            if (!isConfirm)
                                                            {
                                                                return;
                                                            }
                                                            ApiResponse response = await TransactionApi.SendToAddress(argument.Values[0], long.Parse(argument.Values[1]), argument.Values[2], argument.Values[3], bool.Parse(argument.Values[4]));
                                                            if (!response.HasError)
                                                            {
                                                                command.Out.WriteLine(response.Result);
                                                                return;
                                                            }
                                                            else
                                                            {
                                                                command.Out.WriteLine(response.Error.Message);
                                                                return;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        command.Out.WriteLine("invalid parameter");
                                                        return;
                                                    }
                                                }
                                                else
                                                {
                                                    command.Out.WriteLine("invalid address");
                                                    return;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                command.Out.WriteLine(ex.ToString());
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(blockChainResponse.Error.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("transaction to give address command need five paramters");
                                        return;
                                    }

                                #endregion

                                #region set transaction fee

                                case "-f":
                                    if (argument != null && argument.Values.Count == 1)
                                    {
                                        if (long.TryParse(argument.Values[0], out long transactionFeePerKilobyte))
                                        {
                                            ApiResponse response = await TransactionApi.SetTxFee(transactionFeePerKilobyte);
                                            if (!response.HasError)
                                            {
                                                command.Out.WriteLine("execute successfully");
                                                return;
                                            }
                                            else
                                            {
                                                command.Out.WriteLine("invalid parameter");
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            command.Out.WriteLine("invalid parameter");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("set transaction fee command need one paramter");
                                        return;
                                    }

                                #endregion

                                #region set transaction confirmations

                                case "-c":
                                    if (argument != null && argument.Values.Count == 1)
                                    {
                                        if (long.TryParse(argument.Values[0], out long confirmations))
                                        {
                                            ApiResponse response = await TransactionApi.SetTxFee(confirmations);
                                            if (!response.HasError)
                                            {
                                                command.Out.WriteLine("execute successfully");
                                                return;
                                            }
                                            else
                                            {
                                                command.Out.WriteLine("invalid parameter");
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            command.Out.WriteLine("invalid parameter");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("set transaction confirmations command need one paramter");
                                        return;
                                    }

                                #endregion

                                #region get transaction settings

                                case "-s":
                                    if (argument != null && argument.Values.Count == 0)
                                    {
                                        ApiResponse response = await TransactionApi.GetTxSettings();
                                        if (!response.HasError)
                                        {
                                            JToken result = response.Result;
                                            command.Out.WriteLine(result);
                                            return;
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(response.Error.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("get transaction settings command need none paramters");
                                        return;
                                    }

                                #endregion

                                #region estimate tx fee for send to address

                                case "-e":
                                    if (argument != null && argument.Values.Count == 5)
                                    {
                                        ApiResponse blockChainResponse = await BlockChainEngineApi.GetBlockChainStatus();
                                        if (!blockChainResponse.HasError)
                                        {
                                            BlockChainStatus blockChainStatus = blockChainResponse.GetResult<BlockChainStatus>();
                                            //验证address
                                            try
                                            {
                                                if (AddressTools.AddressVerfy(blockChainStatus.ChainNetwork, argument.Values[0]))
                                                {
                                                    if (argument.Values[4] == "true" || argument.Values[4] == "false")
                                                    {
                                                        bool isEncrypt = await IsEncrypt();
                                                        if (isEncrypt)
                                                        {
                                                            string password = Prompt.GetPassword("please input your password to unlock:");
                                                            if (!IsPasswordValid(password))
                                                            {
                                                                command.Out.WriteLine("password is invalid");
                                                                return;
                                                            }
                                                            else
                                                            {
                                                                ApiResponse walletResponse = await WalletManagementApi.WalletPassphrase(password);
                                                                if (walletResponse.HasError)
                                                                {
                                                                    command.Out.WriteLine("password is incorrect");
                                                                    return;
                                                                }
                                                                ApiResponse response = await TransactionApi.EstimateTxFeeForSendToAddress(argument.Values[0], long.Parse(argument.Values[1]), argument.Values[2], argument.Values[3], bool.Parse(argument.Values[4]));
                                                                if (!response.HasError)
                                                                {
                                                                    JToken result = response.Result;
                                                                    command.Out.WriteLine(result);
                                                                    ApiResponse lockResponse = await WalletManagementApi.WalletLock();
                                                                    if (lockResponse.HasError)
                                                                    {
                                                                        command.Out.WriteLine("lock failure");
                                                                    }
                                                                    return;
                                                                }
                                                                else
                                                                {
                                                                    command.Out.WriteLine(response.Error.Message);
                                                                    return;
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            ApiResponse response = await TransactionApi.EstimateTxFeeForSendToAddress(argument.Values[0], long.Parse(argument.Values[1]), argument.Values[2], argument.Values[3], bool.Parse(argument.Values[4]));
                                                            if (!response.HasError)
                                                            {
                                                                JToken result = response.Result;
                                                                command.Out.WriteLine(result);
                                                                return;
                                                            }
                                                            else
                                                            {
                                                                command.Out.WriteLine(response.Error.Message);
                                                                return;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        command.Out.WriteLine("invalid parameter");
                                                        return;
                                                    }
                                                }
                                                else
                                                {
                                                    command.Out.WriteLine("invalid address");
                                                    return;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                command.Out.WriteLine(ex.ToString());
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(blockChainResponse.Error.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("estimate tx fee for send to address command need one paramter");
                                        return;
                                    }

                                #endregion

                                #region estimate tx fee for send many address

                                case "-t":
                                    if (argument != null && argument.Values.Count == 3)
                                    {
                                        //这个需要传入一个Json数组参数，最复杂的参数
                                        SendManyModel[] model = null;
                                        string[] subtractFeeFromAmount = null;
                                        try
                                        {
                                            model = Newtonsoft.Json.JsonConvert.DeserializeObject<SendManyModel[]>(argument.Values[1]);
                                            if (!string.IsNullOrEmpty(argument.Values[2]))
                                            {
                                                subtractFeeFromAmount = argument.Values[2].Split(',');
                                            }
                                            //循环判断地址是否合法
                                            ApiResponse blockChainResponse = await BlockChainEngineApi.GetBlockChainStatus();
                                            if (!blockChainResponse.HasError)
                                            {
                                                BlockChainStatus blockChainStatus = blockChainResponse.GetResult<BlockChainStatus>();
                                                //验证address
                                                for (int i = 0; i < model.Length; i++)
                                                {
                                                    if (!AddressTools.AddressVerfy(blockChainStatus.ChainNetwork, model[i].Address))
                                                    {
                                                        command.Out.WriteLine("invalid address format");
                                                        return;
                                                    }
                                                }
                                                for (int j = 0; j < subtractFeeFromAmount.Length; j++)
                                                {
                                                    if (!AddressTools.AddressVerfy(blockChainStatus.ChainNetwork, subtractFeeFromAmount[j]))
                                                    {
                                                        command.Out.WriteLine("invalid address format");
                                                        return;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                command.Out.WriteLine(blockChainResponse.Error.Message);
                                                return;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            command.Out.WriteLine(ex.ToString());
                                            return;
                                        }
                                        bool isEncrypt = await IsEncrypt();
                                        if (isEncrypt)
                                        {
                                            string password = Prompt.GetPassword("please input your password to unlock:");
                                            if (!IsPasswordValid(password))
                                            {
                                                command.Out.WriteLine("password is invalid");
                                                return;
                                            }
                                            else
                                            {
                                                ApiResponse walletResponse = await WalletManagementApi.WalletPassphrase(password);
                                                if (walletResponse.HasError)
                                                {
                                                    command.Out.WriteLine("password is incorrect");
                                                    return;
                                                }
                                                ApiResponse response = await TransactionApi.EstimateTxFeeForSendMany(argument.Values[0], model, subtractFeeFromAmount);
                                                if (!response.HasError)
                                                {
                                                    JToken result = response.Result;
                                                    command.Out.WriteLine(result);
                                                    ApiResponse lockResponse = await WalletManagementApi.WalletLock();
                                                    if (lockResponse.HasError)
                                                    {
                                                        command.Out.WriteLine("lock failure");
                                                    }
                                                    return;
                                                }
                                                else
                                                {
                                                    command.Out.WriteLine(response.Error.Message);
                                                    return;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ApiResponse response = await TransactionApi.EstimateTxFeeForSendMany(argument.Values[0], model, subtractFeeFromAmount);
                                            if (!response.HasError)
                                            {
                                                JToken result = response.Result;
                                                command.Out.WriteLine(result);
                                                return;
                                            }
                                            else
                                            {
                                                command.Out.WriteLine(response.Error.Message);
                                                return;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("estimate tx fee for send many address command need one paramter");
                                        return;
                                    }

                                #endregion

                                #region list transactions

                                case "-l":
                                    if (argument != null && argument.Values.Count == 4)
                                    {
                                        if (long.TryParse(argument.Values[1], out long count))
                                        {
                                            if (int.TryParse(argument.Values[2], out int skip))
                                            {
                                                if (bool.TryParse(argument.Values[3], out bool includeWatchOnly))
                                                {
                                                    ApiResponse response = await TransactionApi.ListTransactions(argument.Values[0], count, skip, includeWatchOnly);
                                                    if (!response.HasError)
                                                    {
                                                        JToken result = response.Result;
                                                        command.Out.WriteLine(result);
                                                        return;
                                                    }
                                                    else
                                                    {
                                                        command.Out.WriteLine(response.Error.Message);
                                                        return;
                                                    }
                                                }
                                                else
                                                {
                                                    command.Out.WriteLine("invalid parameter");
                                                    return;
                                                }
                                            }
                                            else
                                            {
                                                command.Out.WriteLine("invalid parameter");
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            command.Out.WriteLine("invalid parameter");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("set transaction fee command need four paramters");
                                        return;
                                    }

                                #endregion

                                #region send raw transaction

                                case "-r":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 5)
                                    {
                                        //这个需要传入一个Json数组参数，最复杂的参数
                                        SendRawTransactionInputsIM[] senders = null;
                                        SendRawTransactionOutputsIM[] receivers = null;
                                        try
                                        {
                                            senders = Newtonsoft.Json.JsonConvert.DeserializeObject<SendRawTransactionInputsIM[]>(argument.Values[0]);
                                            receivers = Newtonsoft.Json.JsonConvert.DeserializeObject<SendRawTransactionOutputsIM[]>(argument.Values[1]);
                                            //循环判断地址是否合法
                                            ApiResponse blockChainResponse = await BlockChainEngineApi.GetBlockChainStatus();
                                            if (!blockChainResponse.HasError)
                                            {
                                                BlockChainStatus blockChainStatus = blockChainResponse.GetResult<BlockChainStatus>();
                                                //验证address
                                                for (int i = 0; i < receivers.Length; i++)
                                                {
                                                    if (!AddressTools.AddressVerfy(blockChainStatus.ChainNetwork, receivers[i].Address))
                                                    {
                                                        command.Out.WriteLine("invalid address format");
                                                        return;
                                                    }
                                                }
                                                //找零地址
                                                if (!AddressTools.AddressVerfy(blockChainStatus.ChainNetwork, argument.Values[2]))
                                                {
                                                    command.Out.WriteLine("invalid address format");
                                                    return;
                                                }

                                            }
                                            else
                                            {
                                                command.Out.WriteLine(blockChainResponse.Error.Message);
                                                return;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            command.Out.WriteLine(ex.ToString());
                                            return;
                                        }
                                        bool isEncrypt = await IsEncrypt();
                                        if (isEncrypt)
                                        {
                                            string password = Prompt.GetPassword("please input your password to unlock:");
                                            if (!IsPasswordValid(password))
                                            {
                                                command.Out.WriteLine("password is invalid");
                                                return;
                                            }
                                            else
                                            {
                                                ApiResponse walletResponse = await WalletManagementApi.WalletPassphrase(password);
                                                if (walletResponse.HasError)
                                                {
                                                    command.Out.WriteLine("password is incorrect");
                                                    return;
                                                }
                                                bool isConfirm = Prompt.GetYesNo("confirm your transaction, do your want to continue:", true);
                                                if (!isConfirm)
                                                {
                                                    return;
                                                }
                                                ApiResponse response = await TransactionApi.SendRawTransaction(senders, receivers, argument.Values[2], Convert.ToInt64(argument.Values[3]), Convert.ToInt64(argument.Values[4]));
                                                if (!response.HasError)
                                                {
                                                    command.Out.WriteLine(response.Result);
                                                    ApiResponse lockResponse = await WalletManagementApi.WalletLock();
                                                    if (lockResponse.HasError)
                                                    {
                                                        command.Out.WriteLine("lock failure");
                                                    }
                                                    return;
                                                }
                                                else
                                                {
                                                    command.Out.WriteLine(response.Error.Message);
                                                    return;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            bool isConfirm = Prompt.GetYesNo("confirm your transaction, do your want to continue:", true);
                                            if (!isConfirm)
                                            {
                                                return;
                                            }
                                            ApiResponse response = await TransactionApi.SendRawTransaction(senders, receivers, argument.Values[2], Convert.ToInt64(argument.Values[3]), Convert.ToInt64(argument.Values[4]));
                                            if (!response.HasError)
                                            {
                                                command.Out.WriteLine(response.Result);
                                                return;
                                            }
                                            else
                                            {
                                                command.Out.WriteLine(response.Error.Message);
                                                return;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("send raw transaction command need three paramters");
                                        return;
                                    }

                                #endregion

                                #region estimate raw transactions

                                case "-i":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 4)
                                    {
                                        //这个需要传入一个Json数组参数，最复杂的参数
                                        SendRawTransactionInputsIM[] senders = null;
                                        SendRawTransactionOutputsIM[] receivers = null;
                                        try
                                        {
                                            senders = Newtonsoft.Json.JsonConvert.DeserializeObject<SendRawTransactionInputsIM[]>(argument.Values[0]);
                                            receivers = Newtonsoft.Json.JsonConvert.DeserializeObject<SendRawTransactionOutputsIM[]>(argument.Values[1]);
                                            //循环判断地址是否合法
                                            ApiResponse blockChainResponse = await BlockChainEngineApi.GetBlockChainStatus();
                                            if (!blockChainResponse.HasError)
                                            {
                                                BlockChainStatus blockChainStatus = blockChainResponse.GetResult<BlockChainStatus>();
                                                //验证address
                                                for (int i = 0; i < receivers.Length; i++)
                                                {
                                                    if (!AddressTools.AddressVerfy(blockChainStatus.ChainNetwork, receivers[i].Address))
                                                    {
                                                        command.Out.WriteLine("invalid address format");
                                                        return;
                                                    }
                                                }
                                                //找零地址
                                                if (!AddressTools.AddressVerfy(blockChainStatus.ChainNetwork, argument.Values[2]))
                                                {
                                                    command.Out.WriteLine("invalid address format");
                                                    return;
                                                }

                                            }
                                            else
                                            {
                                                command.Out.WriteLine(blockChainResponse.Error.Message);
                                                return;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            command.Out.WriteLine(ex.ToString());
                                            return;
                                        }
                                        bool isEncrypt = await IsEncrypt();
                                        if (isEncrypt)
                                        {
                                            string password = Prompt.GetPassword("please input your password to unlock:");
                                            if (!IsPasswordValid(password))
                                            {
                                                command.Out.WriteLine("password is invalid");
                                                return;
                                            }
                                            else
                                            {
                                                ApiResponse walletResponse = await WalletManagementApi.WalletPassphrase(password);
                                                if (walletResponse.HasError)
                                                {
                                                    command.Out.WriteLine("password is incorrect");
                                                    return;
                                                }
                                                bool isConfirm = Prompt.GetYesNo("confirm your transaction, do your want to continue:", true);
                                                if (!isConfirm)
                                                {
                                                    return;
                                                }
                                                ApiResponse response = await TransactionApi.EstimateRawTransaction(senders, receivers, argument.Values[2], Convert.ToInt64(argument.Values[3]));
                                                if (!response.HasError)
                                                {
                                                    command.Out.WriteLine(response.Result);
                                                    ApiResponse lockResponse = await WalletManagementApi.WalletLock();
                                                    if (lockResponse.HasError)
                                                    {
                                                        command.Out.WriteLine("lock failure");
                                                    }
                                                    return;
                                                }
                                                else
                                                {
                                                    command.Out.WriteLine(response.Error.Message);
                                                    return;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            bool isConfirm = Prompt.GetYesNo("confirm your transaction, do your want to continue:", true);
                                            if (!isConfirm)
                                            {
                                                return;
                                            }
                                            ApiResponse response = await TransactionApi.EstimateRawTransaction(senders, receivers, argument.Values[2], Convert.ToInt64(argument.Values[3]));
                                            if (!response.HasError)
                                            {
                                                command.Out.WriteLine(response.Result);
                                                return;
                                            }
                                            else
                                            {
                                                command.Out.WriteLine(response.Error.Message);
                                                return;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("estimate raw transactions command need three paramters");
                                        return;
                                    }

                                #endregion

                                #region List since block

                                case "-n":
                                    if (argument != null && argument.Values.Count == 2)
                                    {
                                        if (int.TryParse(argument.Values[1], out int confirmations))
                                        {
                                            ApiResponse response = await TransactionApi.ListSinceBlock(argument.Values[0], confirmations);
                                            if (!response.HasError)
                                            {
                                                JToken result = response.Result;
                                                command.Out.WriteLine(result);
                                                return;
                                            }
                                            else
                                            {
                                                command.Out.WriteLine(response.Error.Message);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            command.Out.WriteLine("invalid parameter");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("List since block command need four paramters");
                                        return;
                                    }

                                #endregion

                                #region get transaction

                                case "-d":
                                    if (argument != null && argument.Values.Count == 1)
                                    {
                                        ApiResponse response = await TransactionApi.GetTransaction(argument.Values[0]);
                                        if (!response.HasError)
                                        {
                                            JToken result = response.Result;
                                            command.Out.WriteLine(result);
                                            return;
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(response.Error.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("get transaction command need none paramters");
                                        return;
                                    }

                                #endregion

                                default:
                                    command.ShowHelp();
                                    return;
                            }
                        }
                        else
                        {
                            command.ShowHelp();
                            return;
                        }
                    });

                });

                #endregion

                #region utxo command

                app.Command("utxo", command =>
                {
                    command.Description = "unspent transaction output operation";
                    command.HelpOption("-?|-h|--help");

                    CommandOption iOption = command.Option("-i", "get tx out set info", CommandOptionType.NoValue);
                    CommandOption lOption = command.Option("-l", "list unspent transaction output", CommandOptionType.NoValue);
                    CommandOption bOption = command.Option("-b", "get unconfirmed balance", CommandOptionType.NoValue);
                    CommandOption oOption = command.Option("-o", "get tx out", CommandOptionType.NoValue);
                    CommandOption pOption = command.Option("-p", "list page unspent", CommandOptionType.NoValue);

                    CommandArgument argument = command.Argument("arguments", "execute some option need params", multipleValues: true);
                    command.OnExecute(async () =>
                    {
                        //在这里使用上面各种 Argument 和 Option 的 Value 或 Values 属性拿值。
                        if (args != null && args.Length > 1)
                        {
                            switch ((args[args.Length - 1]).ToLower())
                            {
                                #region get tx out set info

                                case "-i":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 0)
                                    {
                                        ApiResponse response = await UtxoApi.GetTxOutSetInfo();

                                        if (!response.HasError)
                                        {
                                            JToken result = response.Result;
                                            command.Out.WriteLine(result);
                                            return;
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(response.Error.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("get tx out set info command need none paramters");
                                        return;
                                    }

                                #endregion

                                #region list unspent transaction output

                                case "-l":
                                    //判断参数个数调用接口
                                    if (argument != null)
                                    {
                                        if (argument.Values.Count == 1)
                                        {
                                            int minConfirmations = 0;
                                            if (!int.TryParse(argument.Values[0], out minConfirmations))
                                            {
                                                command.Out.WriteLine("invalid parameter");
                                                return;
                                            }
                                            ApiResponse response = await UtxoApi.ListUnspent(minConfirmations);
                                            if (!response.HasError)
                                            {
                                                JToken result = response.Result;
                                                command.Out.WriteLine(result);
                                                return;
                                            }
                                            else
                                            {
                                                command.Out.WriteLine(response.Error.Message);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            int minConfirmations = 0;
                                            int maxConfirmations = 0;
                                            if (!int.TryParse(argument.Values[0], out minConfirmations))
                                            {
                                                command.Out.WriteLine("invalid parameter");
                                                return;
                                            }

                                            if (!int.TryParse(argument.Values[1], out maxConfirmations))
                                            {
                                                command.Out.WriteLine("invalid parameter");
                                                return;
                                            }
                                            string[] address = null;
                                            if (argument.Values[2] != null)
                                            {
                                                address = argument.Values[2].Split(',');
                                                ApiResponse blockChainResponse = await BlockChainEngineApi.GetBlockChainStatus();
                                                if (!blockChainResponse.HasError)
                                                {
                                                    BlockChainStatus blockChainStatus = blockChainResponse.GetResult<BlockChainStatus>();
                                                    //验证address
                                                    for (int i = 0; i < address.Length; i++)
                                                    {
                                                        if (!AddressTools.AddressVerfy(blockChainStatus.ChainNetwork, address[i]))
                                                        {
                                                            command.Out.WriteLine("invalid parameters");
                                                        }
                                                    }
                                                }
                                            }
                                            ApiResponse response = await UtxoApi.ListUnspent(minConfirmations, maxConfirmations, address);
                                            if (!response.HasError)
                                            {
                                                JToken result = response.Result;
                                                command.Out.WriteLine(result);
                                                return;
                                            }
                                            else
                                            {
                                                command.Out.WriteLine(response.Error.Message);
                                                return;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("list unspent transaction output command need three paramter");
                                        return;
                                    }

                                #endregion

                                #region get unconfirmed balance

                                case "-b":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 0)
                                    {
                                        ApiResponse response = await UtxoApi.GetUnconfirmedBalance();
                                        if (!response.HasError)
                                        {
                                            JToken result = response.Result;
                                            command.Out.WriteLine(result);
                                            return;
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(response.Error.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("get unconfirmed balance command need none paramters");
                                        return;
                                    }

                                #endregion

                                #region get tx out

                                case "-o":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 3)
                                    {
                                        int vout = 0;
                                        bool isConfirmed = false;
                                        if (!int.TryParse(argument.Values[1], out vout))
                                        {
                                            command.Out.WriteLine("invalid parameter");
                                            return;
                                        }
                                        if (!bool.TryParse(argument.Values[2], out isConfirmed))
                                        {
                                            command.Out.WriteLine("invalid parameter");
                                            return;
                                        }
                                        ApiResponse response = await UtxoApi.GetTxOut(argument.Values[0], vout, isConfirmed);
                                        if (!response.HasError)
                                        {
                                            JToken result = response.Result;
                                            command.Out.WriteLine(result);
                                            return;
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(response.Error.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("get tx out command need three paramters");
                                        return;
                                    }

                                #endregion

                                #region list page unspent

                                case "-p":
                                    //判断参数个数调用接口
                                    if (argument != null && (argument.Values.Count == 3 || argument.Values.Count == 7))
                                    {
                                        long minConfirmations = Convert.ToInt64(argument.Values[0]);
                                        int currentPage = Convert.ToInt32(argument.Values[1]);
                                        int pageSize = Convert.ToInt32(argument.Values[2]);
                                        long maxConfirmations = argument.Values.Count == 3 ? 9999999 : Convert.ToInt64(argument.Values[3]);
                                        long minAmount = argument.Values.Count == 3 ? 1 : Convert.ToInt64(argument.Values[4]);
                                        long maxAmount = argument.Values.Count == 3 ? long.MaxValue : Convert.ToInt64(argument.Values[5]);
                                        bool isDesc = argument.Values.Count != 3;
                                        ApiResponse response = await UtxoApi.ListPageUnspent(minConfirmations, currentPage, pageSize, maxConfirmations, minAmount, maxAmount, isDesc);
                                        if (!response.HasError)
                                        {
                                            JToken result = response.Result;
                                            command.Out.WriteLine(result);
                                            return;
                                        }
                                        else
                                        {
                                            command.Out.WriteLine(response.Error.Message);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("get tx out command paramters count invalid");
                                        return;
                                    }

                                #endregion

                                default:
                                    command.ShowHelp();
                                    return;
                            }
                        }
                        else
                        {
                            command.ShowHelp();
                            return;
                        }
                    });

                });

                #endregion

                #region wallet command

                app.Command("wallet", command =>
                {
                    command.Description = "wallet operation";
                    command.HelpOption("-?|-h|--help");

                    CommandOption bOption = command.Option("-b", "backup wallet", CommandOptionType.NoValue);
                    CommandOption rOption = command.Option("-r", "restore wallet backup", CommandOptionType.NoValue);
                    CommandOption eOption = command.Option("-e", "encrypt wallet", CommandOptionType.NoValue);
                    CommandOption pOption = command.Option("-p", "wallet passphrase", CommandOptionType.NoValue);
                    CommandOption lOption = command.Option("-l", "wallet lock", CommandOptionType.NoValue);
                    CommandOption cOption = command.Option("-c", "wallet password change", CommandOptionType.NoValue);

                    CommandArgument argument = command.Argument("arguments", "execute some option need params", multipleValues: true);
                    command.OnExecute(async () =>
                    {
                        //在这里使用上面各种 Argument 和 Option 的 Value 或 Values 属性拿值。
                        if (args != null && args.Length > 1)
                        {
                            switch ((args[args.Length - 1]).ToLower())
                            {
                                #region backup wallet

                                case "-b":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 1)
                                    {
                                        //先判断钱包是否加密，然后判断输入的扩展名是否合法
                                        bool isEncrypt = await IsEncrypt();
                                        if (isEncrypt)
                                        {
                                            //判断输入的密钥是否合法
                                            string password = Prompt.GetPassword("please input your password:");
                                            ApiResponse validateResponse = await WalletManagementApi.WalletPassphrase(password);
                                            if (!validateResponse.HasError)
                                            {
                                                bool validateResult = validateResponse.GetResult<bool>();
                                                if (validateResult)
                                                {
                                                    if (System.IO.Path.GetExtension(argument.Values[0]) == ".fcdatx")
                                                    {
                                                        ApiResponse response = await WalletManagementApi.BackupWallet(argument.Values[0]);
                                                        if (!response.HasError)
                                                        {
                                                            command.Out.WriteLine("execute successfully");
                                                            return;
                                                        }
                                                        else
                                                        {
                                                            command.Out.WriteLine(response.Error.Message);
                                                            return;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        command.Out.WriteLine("invalid parameter");
                                                        return;
                                                    }
                                                }
                                                else
                                                {
                                                    command.Out.WriteLine("password is incorrect");
                                                    return;
                                                }
                                            }
                                            else
                                            {
                                                command.Out.WriteLine("password is incorrect");
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            if (System.IO.Path.GetExtension(argument.Values[0]) == ".fcdat")
                                            {
                                                ApiResponse response = await WalletManagementApi.BackupWallet(argument.Values[0]);
                                                if (!response.HasError)
                                                {
                                                    command.Out.WriteLine("execute successfully");
                                                    return;
                                                }
                                                else
                                                {
                                                    command.Out.WriteLine(response.Error.Message);
                                                    return;
                                                }
                                            }
                                            else
                                            {
                                                command.Out.WriteLine("invalid parameter");
                                                return;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("backup wallet command need none paramters");
                                        return;
                                    }

                                #endregion

                                #region restore wallet backup

                                case "-r":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 1)
                                    {
                                        //先根据输入的路径判断文件扩展名，如果是加密文件，让用户输入密码解密
                                        string extension = System.IO.Path.GetExtension(argument.Values[0]);
                                        if (extension == ".fcdatx")
                                        {
                                            //判断输入的密钥是否合法
                                            string password = Prompt.GetPassword("please input your password:");
                                            ApiResponse response = await WalletManagementApi.RestoreWalletBackup(argument.Values[0], password);
                                            if (!response.HasError)
                                            {
                                                command.Out.WriteLine("execute successfully");
                                                return;
                                            }
                                            else
                                            {
                                                command.Out.WriteLine(response.Error.Message);
                                                return;
                                            }
                                        }
                                        if (extension == ".fcdat")
                                        {
                                            ApiResponse response = await WalletManagementApi.RestoreWalletBackup(argument.Values[0]);
                                            if (!response.HasError)
                                            {
                                                command.Out.WriteLine("execute successfully");
                                                return;
                                            }
                                            else
                                            {
                                                command.Out.WriteLine(response.Error.Message);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            command.Out.WriteLine("invalid parameter");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("restore wallet backup command need one paramter");
                                        return;
                                    }

                                #endregion

                                #region encrypt wallet

                                case "-e":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 1)
                                    {
                                        bool isEncrypt = await IsEncrypt();
                                        if (isEncrypt)
                                        {
                                            command.Out.WriteLine("cannot repeat encryption");
                                            return;
                                        }
                                        else
                                        {
                                            if (!IsPasswordValid(argument.Values[0]))
                                            {
                                                command.Out.WriteLine("password must more than 8 character,less than 30 character and contain uppercase and lowercase letters, numbers and special symbols");
                                                return;
                                            }
                                            else
                                            {
                                                ApiResponse response = await WalletManagementApi.EncryptWallet(argument.Values[0]);
                                                if (!response.HasError)
                                                {
                                                    command.Out.WriteLine("execute successfully");
                                                    return;
                                                }
                                                else
                                                {
                                                    command.Out.WriteLine(response.Error.Message);
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("encrypt wallet command need none paramters");
                                        return;
                                    }

                                #endregion

                                #region wallet passphrase

                                case "-p":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 1)
                                    {
                                        bool isEncrypt = await IsEncrypt();
                                        if (!isEncrypt)
                                        {
                                            command.Out.WriteLine("wallet is not encrypted");
                                            return;
                                        }
                                        else
                                        {
                                            //validate password
                                            if (IsPasswordValid(argument.Values[0]))
                                            {
                                                ApiResponse response = await WalletManagementApi.WalletPassphrase(argument.Values[0]);
                                                if (!response.HasError)
                                                {
                                                    bool result = response.GetResult<bool>();
                                                    if (result)
                                                    {
                                                        command.Out.WriteLine("execute successfully");
                                                        return;
                                                    }
                                                    else
                                                    {
                                                        command.Out.WriteLine("execute fail");
                                                        return;
                                                    }
                                                }
                                                else
                                                {
                                                    command.Out.WriteLine(response.Error.Message);
                                                    return;
                                                }
                                            }
                                            else
                                            {
                                                command.Out.WriteLine("password is invalid");
                                                return;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("wallet passphrase command need three paramters");
                                        return;
                                    }

                                #endregion

                                #region wallet lock

                                case "-l":
                                    //判断参数个数调用接口
                                    if (argument != null && argument.Values.Count == 0)
                                    {
                                        //判断是否加密
                                        bool isEncrypt = await IsEncrypt();
                                        if (!isEncrypt)
                                        {
                                            command.Out.WriteLine("wallet is not encrypt");
                                            return;
                                        }
                                        else
                                        {
                                            ApiResponse response = await WalletManagementApi.WalletLock();
                                            if (!response.HasError)
                                            {
                                                command.Out.WriteLine("execute successfully");
                                                return;
                                            }
                                            else
                                            {
                                                command.Out.WriteLine(response.Error.Message);
                                                return;
                                            }
                                        }

                                    }
                                    else
                                    {
                                        command.Out.WriteLine("wallet lock command need none paramters");
                                        return;
                                    }

                                #endregion

                                #region wallet password change

                                case "-c":
                                    if (argument != null && argument.Values.Count == 2)
                                    {
                                        bool isEncrypt = await IsEncrypt();
                                        if (!isEncrypt)
                                        {
                                            command.Out.WriteLine("wallet is not encrypted");
                                            return;
                                        }
                                        else
                                        {
                                            string oldPassword = Prompt.GetPassword("please input your password:");
                                            if (!IsPasswordValid(oldPassword))
                                            {
                                                command.Out.WriteLine("password is invalid");
                                                return;
                                            }
                                            else
                                            {
                                                ApiResponse apiResponse = await WalletManagementApi.WalletPassphrase(oldPassword);
                                                if (!apiResponse.HasError)
                                                {
                                                    string newPassword = Prompt.GetPassword("please input new password:");
                                                    if (newPassword != oldPassword)
                                                    {
                                                        string renewPassword = Prompt.GetPassword("please input new password again:");
                                                        if (renewPassword == newPassword)
                                                        {
                                                            ApiResponse response = await WalletManagementApi.WalletPassphraseChange(oldPassword, newPassword);
                                                            if (!response.HasError)
                                                            {
                                                                command.Out.WriteLine("execute successfully");
                                                                return;
                                                            }
                                                            else
                                                            {
                                                                command.Out.WriteLine(response.Error.Message);
                                                                return;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            command.Out.WriteLine("Two input password inconsistencies");
                                                            return;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        command.Out.WriteLine("new password is not same with the old password");
                                                        return;
                                                    }
                                                }
                                                else
                                                {
                                                    command.Out.WriteLine(apiResponse.Error.Message);
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        command.Out.WriteLine("wallet password change command need two paramters");
                                        return;
                                    }

                                #endregion

                                default:
                                    command.ShowHelp();
                                    return;
                            }
                        }
                        else
                        {
                            command.ShowHelp();
                            return;
                        }
                    });

                });

                #endregion

                app.Execute(args);
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                app.Execute(new string[] { "-?" });
            }
        }

        public static async Task<bool> IsEncrypt()
        {
            bool isEncrypt = false;
            //先调用接口判断是否加密
            ApiResponse response = await TransactionApi.GetTxSettings();
            if (!response.HasError)
            {
                TransactionFeeSetting setting = response.GetResult<TransactionFeeSetting>();
                isEncrypt = setting.Encrypt;
            }
            else
            {
                throw new ApiCustomException(response.Error.Code, response.Error.Message);
            }
            return isEncrypt;
        }

        public static bool IsPasswordValid(string password)
        {
            Regex regex = new Regex(@"
                (?=.*[0-9])                     #必须包含数字
                (?=.*[a-z])                     #必须包含小写字母
                (?=.*[A-Z])                     #必须包含大写字母
                (?=([\x21-\x7e]+)[^a-zA-Z0-9])  #必须包含特殊符号
                .{8,30}                         #至少8个字符，最多30个字符
                ", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            if (!regex.IsMatch(password))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static async Task<bool> IsChainRunning()
        {
            ApiResponse api = await BlockChainEngineApi.GetBlockChainStatus();
            if (!api.HasError)
            {
                BlockChainStatus status = api.GetResult<BlockChainStatus>();
                if (status.ChainService == "Running")
                {
                    return true;
                }
            }
            return false;
        }
    }
}
