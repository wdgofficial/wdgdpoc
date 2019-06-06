// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.

namespace WDG.Wallet.Cli.Api
{
    public class ApiRequestCommand
    {
        /* Api                                       Request
         * account Api
         * 1、GetAddressesByTag                      account "" -t                                                       
         * 2、GetAccountByAddress                    account "fiiitPy8XkrmGYXAxHnnQuj16u6ifjdMkWSfeC" -a                                                
         * 3、GetNewAddress                          account "new tag" -n                                                    
         * 4、GetDefaultAccount                      account -d                                                            
         * 5、SetDefaultAccount                      account "fiiitPy8XkrmGYXAxHnnQuj16u6ifjdMkWSfeC" -s                                                 
         * 6、ValidateAddress                        account "fiiitPy8XkrmGYXAxHnnQuj16u6ifjdMkWSfeC" -v                     
         * 7、SetAccountTag                          account "fiiitPy8XkrmGYXAxHnnQuj16u6ifjdMkWSfeC" "new tag" -c                          

         * address Api
         * 1、AddNewAddressBookItem                  address "fiiitPy8XkrmGYXAxHnnQuj16u6ifjdMkWSfeC" "label" -a                                
         * 2、GetAddressBook                         address -g                                                    
         * 3、GetAddressBookItemByAddress            address "fiiitPy8XkrmGYXAxHnnQuj16u6ifjdMkWSfeC" -b                                         
         * 4、GetAddressBookByTag                    address "tag" -t                                               
         * 5、DeleteAddressBookByIds                 address "3,4,5,6" -d                                 

         * block Api
         * 1、GetBlockChainStatus                    block -u                                        
         * 2、StopEngine                             block -n                                        
         * 3、GetBlockCount                          block -c                                        
         * 4、GetBlockHash                           block 20 -i                                 
         * 5、GetBlock                               block "72E7D993FE2CF101640968A8FE6B4E381017719154454EF8580373D2F6697935" 1 -b                         
         * 6、GetBlockHeader                         block "72E7D993FE2CF101640968A8FE6B4E381017719154454EF8580373D2F6697935" 1 -e                       
         * 7、GetChainTips                           block -t                                        
         * 8、GetDifficulty                          block -d                                        
         * 9、GenerateNewBlock                       block "wangshibang" "fiiitPy8XkrmGYXAxHnnQuj16u6ifjdMkWSfeC" 1 -g           
         * 10、SubmitBlock                           block "1234567890ABCDEF" -s

         * memory Api
         * 1、GetAllTxInMemPool                      memory -a                           
         * 2、GetPaymentInfoInMemPool                memory "8032204179D57C913899205374C47FCA8BBB3970C55083334CC8483EA1040C2D" -p                           

         * net Api
         * 1、GetNetworkInfo                         net -i                                          
         * 2、GetNetTotals                           net -t                                          
         * 3、GetConnectionCount                     net -s                                          
         * 4、GetPeerInfo                            net -p                                          
         * 5、AddNode                                net "192.168.1.177:4321 "-a                      
         * 6、GetAddedNodeInfo                       net "192.168.1.177:4321" -n                      
         * 7、DisconnectNode                         net "192.168.1.177:4321" -d                      
         * 8、SetBan                                 net "192.168.1.177:4321" "add" -b   (the second parameter only have add and remove two values)            
         * 9、ListBanned                             net -l                                          
         * 10、ClearBanned                           net -c                                          
         * 11、SetNetworkActive                      net true -v                               
         * 12、GetBlockChainInfo                     net -o                                               

         * pay Api
         * 1、CreateNewPaymentRequest                pay "tag" 11.11 "comment" -c                    
         * 2、GetAllPaymentRequests                  pay -a                                          
         * 3、DeletePaymentRequestsByIds             pay "3,7,8,9" -d                        

         * trans Api
         * 1、SendMany                               trans "fiiitPy8XkrmGYXAxHnnQuj16u6ifjdMkWSfeC" [{"address":"111QArGP3ufFtLYACfAHHaGjP1p7FuXdNQ","tag":"john","amount":100000,"comment":"no comment"},{"address":"122QArGP3ufFtLYACfAHHaGjP1p7FuXdND","tag":"john","amount":100000,"comment":"no comment"}] "1200000000,1400000000" -m        
         * 2、SendToAddress                          trans "fiiit7SG73JrwyndCoZCYKcbvpZaSSpxfCt7to" 12000 "this is comment" "this is commentTo" true -a                  
         * 3、SetTxFee                               trans 100 -f                                                           
         * 4、SetConfirmations                       trans 90 -c                                                                     
         * 5、GetTxSettings                          trans -s                                                                                    
         * 6、EstimateTxFeeForSendToAddress          trans "fiiit7SG73JrwyndCoZCYKcbvpZaSSpxfCt7to" 10000 "this is comment" "this is commentTo" true -e                  
         * 7、EstimateTxFeeForSendMany               trans -t "fiiitPy8XkrmGYXAxHnnQuj16u6ifjdMkWSfeC" "fiiitPy8XkrmGYXAxHnnQuj16u6ifjdMkWSfeC,fiiit7SG73JrwyndCoZCYKcbvpZaSSpxfCt7to" "12000,14000"         
         * 8、ListTransactions                       trans "*" 10 10 true -l     (the first parameter stands for all account, the second stands for count, the third stands for skip, the last stands for include watch only)

         * utxo Api
         * 1、GetTxOutSetInfo                        utxo -i                                                                         
         * 2、ListUnspent                            utxo 1 6 "fiiitPy8XkrmGYXAxHnnQuj16u6ifjdMkWSfeC,fiiit7SG73JrwyndCoZCYKcbvpZaSSpxfCt7to" -l       
         * 3、GetUnconfirmedBalance                  utxo -b                                                                         
         * 4、GetTxOut                               utxo "8032204179D57C913899205374C47FCA8BBB3970C55083334CC8483EA1040C2D" 0 false -o 

         * wallet Api
         * 1、BackupWallet                           wallet "D:\\wallet.fcdat" -b                            
         * 2、RestoreWalletBackup                    wallet "D:\\wallet.fcdat" -r                             
         * 3、EncryptWallet                          wallet "P@ssw0rd$" -e                            
         * 4、WalletPassphrase                       wallet "P@ssw0rd$" -p                            
         * 5、WalletLock                             wallet -l                                       
         * 6、WalletPassphraseChange                 wallet "P@ssw0rd$" "newP@ssw0rd$" -c 
         * 
         */
    }
}
