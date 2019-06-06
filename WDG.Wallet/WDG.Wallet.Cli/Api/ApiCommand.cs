// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.


namespace WDG.Wallet.Cli.Api
{
    public class ApiCommand
    {
        /* 命令行科普
         * 例如输入： trans 123 456 789 -r 123 -r 789
         * 上面例子中：trans是Command，123 456 789是CommandArgument，-r之后的都是CommandOption.注意：命令行的格式是固定的
         * Command是必须的，CommandArgument和CommandOption都是可选的
         * 只有设置了CommandArgument的multipleValues为true后，CommandArgument才可以接受多个参数，单个参数和多个参数可以通过CommandArgument.Values获取
         * CommandOption设置了MultipleValue之后输入格式必须为-option optionvalue -option optionvalue...
         * 注意我们这里的输入格式为 Command 参数1 参数2 ... -Option，我们这里的option都是 novalue的
         * 
         */

        /* account command                                                                                                                                   Remark
         * 1、GetAddressesByTag                      account "tag"           -t                                                                              根据Tag获取所有账户信息(可以输入空字符串获取所有)                                
         * 2、GetAccountByAddress                    account "address"       -a                                                                              根据地址获取单个账户信息                               
         * 3、GetNewAddress                          account "tag"           -n                                                                              生成一个新的账户信息                                   
         * 4、GetDefaultAccount                      account                 -d                                                                              获取默认账户信息                                       
         * 5、SetDefaultAccount                      account "address"       -s                                                                              设置默认账号                                          
         * 6、ValidateAddress                        account "address"       -v                                                                              验证地址格式
         * 7、SetAccountTag                          account "address" "tag" -c                                                                              设置账户标签

         * address command
         * 1、AddNewAddressBookItem                  address "address" "label"    -a                                                                         地址簿中添加新项
         * 2、GetAddressBook                         address                      -g                                                                         获取地址簿中信息
         * 3、GetAddressBookItemByAddress            address "address"            -b                                                                         通过地址获取地址簿中项
         * 4、GetAddressBookByTag                    address "tag"                -t                                                                         通过Tag获取地址簿中项
         * 5、DeleteAddressBookByIds                 address "id1,id2,id3,id4..." -d                                                                         ids批量删除地址簿中项（id用逗号分隔）

         * block command
         * 1、GetBlockChainStatus                    block                               -u                                                                  获取BlockChain状态
         * 2、StopEngine                             block                               -n                                                                  停止Block引擎
         * 3、GetBlockCount                          block                               -c                                                                  获取区块高度
         * 4、GetBlockHash                           block height                        -i                                                                  根据高度获取区块哈希值
         * 5、GetBlock                               block "hash" formate(暂时只能用1)    -b                                                                  根据哈希值和格式获取区块内容
         * 6、GetBlockHeader                         block "hash" formate(暂时只能用1)    -e                                                                  根据哈希值和区块获取区块头信息
         * 7、GetChainTips                           block                               -t                                                                  获取区块链中分叉的区块端点
         * 8、GetDifficulty                          block                               -d                                                                  获取下个区块的难度
         * 9、GenerateNewBlock                       block "minerName" "address" formate -g                                                                  生成一个新的区块
         * 10、SubmitBlock                           block "blockData"                   -s                                                                  提交一个新的区块
         * 11、SignMessage                           block address, message              -m                                                                  消息进行签名（加密需要先解密）

         * memory command
         * 1、GetAllTxInMemPool                      memory        -a                                                                                        返回内存池中所有交易记录Hash值
         * 2、GetPaymentInfoInMemPool                memory "txid" -p                                                                                        返回内存池中交易详情信息

         * net command
         * 1、GetNetworkInfo                         net                               -i                                                                    获取本节点网络版本信息
         * 2、GetNetTotals                           net                               -t                                                                    获取本节点网络传输统计数据
         * 3、GetConnectionCount                     net                               -s                                                                    当前连接的节点数量
         * 4、GetPeerInfo                            net                               -p                                                                    获取当前连接对等节点的详细信息
         * 5、AddNode                                net "ipAddressWithPort"           -a                                                                    新增节点连接
         * 6、GetAddedNodeInfo                       net "ipAddressWithPort"           -n                                                                    获取添加的节点信息
         * 7、DisconnectNode                         net "ipAddressWithPort"           -d                                                                    断开节点连接
         * 8、SetBan                                 net "ipAddressWithPort" "command" -b                                                                    将节点添加到黑名单或从黑名单删除
         * 9、ListBanned                             net                               -l                                                                    列出所有黑名单
         * 10、ClearBanned                           net                               -c                                                                    清空所有黑名单
         * 11、SetNetworkActive                      net isActivity                    -v                                                                    将网络状态设置为激活或者不激活
         * 12、GetBlockChainInfo                     net                               -o                                                                    获取区块链上的网络状态与区块信息

         * pay command
         * 1、CreateNewPaymentRequest                pay "tag" amount "comment" -c                                                                           创建新的付款请求
         * 2、GetAllPaymentRequests                  pay                        -a                                                                           获取所有付款请求记录
         * 3、DeletePaymentRequestsByIds             pay "id1,id2,id3,id4..."   -d                                                                           根据id删除付款请求

         * trans command
         * 1、SendMany                               trans "fromAccount" "address1,address2,address3..." "amount1,amount2,amount3..." -m                     同时转账给多个地址    
         * 2、SendToAddress                          trans "address" "amount" "comment" "commentTo" "IsSubtractFeeFromAmount"         -a                     转账到单个账户地址
         * 3、SetTxFee                               trans transactionFeePerKilobyte                                                  -f                     设置交易费率
         * 4、SetConfirmations                       trans confirmations                                                              -c                     设置钱包中区块的标准确认次数
         * 5、GetTxSettings                          trans                                                                            -s                     获取钱包中的交易设置
         * 6、EstimateTxFeeForSendToAddress          trans "address" "amount" "comment" "commentTo" "IsSubtractFeeFromAmount"         -e                     计算交易费用1
         * 7、EstimateTxFeeForSendMany               trans "fromAccount" "address1,address2,address3..." "amount1,amount2,amount3..." -t                     计算交易费用2
         * 8、ListTransactions                       trans "account" count skip isIncludeWatchOnly                                    -l                     查询钱包自己相关的交易记录
         * 9、GetTransaction                         trans txid                                                                       -d                     根据交易Id获取交易信息
         * 10、ListSinceBlock                        trans blockhash confirmations                                                    -n

         * utxo command
         * 1、GetTxOutSetInfo                        utxo                                                                   -i                               获取UTXOSet信息
         * 2、ListUnspent                            utxo minConfirmations maxConfirmations "address1,address2,address3..." -l                               列出未花费的UTXO, 最大确认次数和
         * 3、GetUnconfirmedBalance                  utxo                                                                   -b                               获取等待确认的金额
         * 4、GetTxOut                               utxo "txid" vout "isConfirmed"                                         -o                               获取指定的UTXO
         * 5、ListPageUnspent                        utxo minConfirmations currentPage pageSize maxConfirmations minAmount maxAmount isDesc -p               分页获取

         * wallet command
         * 1、BackupWallet                           wallet "filePath"                  -b                                                                   备份钱包
         * 2、RestoreWalletBackup                    wallet "filePath"                  -r                                                                   恢复/还原钱包备份
         * 3、EncryptWallet                          wallet "password"                  -e                                                                   加密钱包
         * 4、WalletPassphrase                       wallet "password"                  -p                                                                   使用密码解锁钱包
         * 5、WalletLock                             wallet                             -l                                                                   锁定钱包
         * 6、WalletPassphraseChange                 wallet "oldPassword" "newPassword" -c                                                                   修改钱包密码
         */
    }
}
