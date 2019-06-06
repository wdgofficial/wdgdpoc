// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

using WDG.DTO;
using WDG.Models;
using WDG.ServiceAgent;
using WDG.Utility;
using WDG.Utility.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDG.Business
{
    public static class NetworkApi
    {
        public static async Task<ApiResponse> GetNetworkInfo()
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Network network = new Network();
                NetworkInfo list = new NetworkInfo();
                NetworkInfoOM result = await network.GetNetworkInfo();
                if (result != null)
                {
                    list.Connections = result.Connections;
                    list.IsRunning = result.IsRunning;
                    list.MinimumSupportedVersion = result.MinimumSupportedVersion;
                    list.ProtocolVersion = result.ProtocolVersion;
                    list.Version = result.Version;

                    response.Result = Newtonsoft.Json.Linq.JToken.FromObject(list);
                }
                else
                {
                    response.Result = null;
                }
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }

        public static async Task<ApiResponse> GetNetTotals()
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Network network = new Network();
                NetTotals totals = new NetTotals();
                NetTotalsOM result = await network.GetNetTotals();
                if (result != null)
                {
                    totals.TimeMillis = result.TimeMillis;
                    totals.TotalBytesRecv = result.TotalBytesRecv;
                    totals.TotalBytesSent = result.TotalBytesSent;

                    response.Result = Newtonsoft.Json.Linq.JToken.FromObject(totals);
                }
                else
                {
                    response.Result = null;
                }
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }

        public static async Task<ApiResponse> GetConnectionCount()
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Network network = new Network();
                long result = await network.GetConnectionCount();

                response.Result = Newtonsoft.Json.Linq.JToken.FromObject(result);
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }

        public static async Task<ApiResponse> GetPeerInfo()
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Network network = new Network();
                List<PeerInfo> list = new List<PeerInfo>();
                PeerInfoOM[] result = await network.GetPeerInfo();
                if (result != null)
                {
                    for (int i = 0; i < result.Length; i++)
                    {
                        list.Add(new PeerInfo()
                        {
                            Addr = result[i].Addr,
                            BytesRecv = result[i].BytesRecv,
                            BytesSent = result[i].BytesSent,
                            ConnTime = result[i].ConnTime,
                            Id = result[i].Id,
                            Inbound = result[i].Inbound,
                            IsTracker = result[i].IsTracker,
                            LastHeartBeat = result[i].LastHeartBeat,
                            LastRecv = result[i].LastRecv,
                            LastSend = result[i].LastSend,
                            LatestHeight = result[i].LatestHeight,
                            Version = result[i].Version
                        });
                    }
                    response.Result = Newtonsoft.Json.Linq.JToken.FromObject(list);
                }
                else
                {
                    response.Result = null;
                }
                
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }

        public static async Task<ApiResponse> AddNode(string ipAddressWithPort)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Network network = new Network();
                await network.AddNode(ipAddressWithPort);
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }

        public static async Task<ApiResponse> GetAddedNodeInfo(string ipAddressWithPort)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Network network = new Network();
                AddNodeInfo node = new AddNodeInfo();
                AddNodeInfoOM result = await network.GetAddedNodeInfo(ipAddressWithPort);
                if (result != null)
                {
                    node.Address = result.Address;
                    node.Connected = result.Connected;
                    node.ConnectedTime = result.ConnectedTime;

                    response.Result = Newtonsoft.Json.Linq.JToken.FromObject(node);
                }
                else
                {
                    response.Result = null;
                }
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }

        public static async Task<ApiResponse> DisconnectNode(string ipAddressWithPort)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Network network = new Network();
                bool result = await network.DisconnectNode(ipAddressWithPort);

                response.Result = Newtonsoft.Json.Linq.JToken.FromObject(result);
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }

        public static async Task<ApiResponse> SetBan(string ipAddressWithPort, string command)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Network network = new Network();
                await network.SetBan(ipAddressWithPort, command);
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }

        public static async Task<ApiResponse> ListBanned()
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Network network = new Network();
                List<BannedInfo> banned = new List<BannedInfo>();
                BannedInfoOM[] result = await network.ListBanned();
                if (result != null)
                {
                    for (int i = 0; i < result.Length; i++)
                    {
                        banned.Add(new BannedInfo()
                        {
                            Address = result[i].Address,
                            Expired = result[i].Expired,
                            Id = result[i].Id,
                            Timestamp = result[i].Timestamp
                        });
                    }

                    response.Result = Newtonsoft.Json.Linq.JToken.FromObject(banned);
                }
                else
                {
                    response.Result = null;
                }
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }

        public static async Task<ApiResponse> ClearBanned()
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Network network = new Network();
                await network.ClearBanned();
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }

        public static async Task<ApiResponse> SetNetworkActive(bool isActivity)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Network network = new Network();
                await network.SetNetworkActive(isActivity);
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }

        public static async Task<ApiResponse> GetBlockChainInfo()
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Network network = new Network();
                BlockChainInfo node = new BlockChainInfo();
                BlockChainInfoOM result = await network.GetBlockChainInfo();
                if (result != null)
                {
                    node.Connections = result.Connections;
                    node.IsRunning = result.IsRunning;
                    node.LocalLastBlockHeight = result.LocalLastBlockHeight;
                    node.LocalLastBlockTime = result.LocalLastBlockTime;
                    node.RemoteLatestBlockHeight = result.RemoteLatestBlockHeight;
                    node.TimeOffset = result.TimeOffset;
                    node.TempBlockCount = result.TempBlockCount;
                    response.Result = Newtonsoft.Json.Linq.JToken.FromObject(node);
                }
                else
                {
                    response.Result = null;
                }
            }
            catch (ApiCustomException ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.ErrorCode, ex.ToString());
            }
            catch (Exception ex)
            {
                Logger.Singleton.Error(ex.ToString());
                response.Error = new ApiError(ex.HResult, ex.ToString());
            }
            return response;
        }
    }
}
