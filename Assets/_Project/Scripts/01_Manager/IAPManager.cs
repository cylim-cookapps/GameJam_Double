using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using CookApps.AnalyticsLite;
using CookApps.Iap;
using CookApps.Iap.Result;
using Pxp.Data;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

namespace Pxp
{
    public class IAPManager : Singleton<IAPManager>
    {
        private UniTaskCompletionSource<List<ItemInfo>> _awaiter;

        public async UniTask OnInitialize()
        {
            List<ProductCatalogItem> items = LocalCreateProductItems();

#if UNITY_EDITOR
            CookAppsIap.UseFakeStoreAlways = true;
            CookAppsIap.UseFakeStoreUIMode = FakeStoreUIMode.Default;
#endif

            //구글, 애플 초기화 파라미터
            var param = new IapInitializeParam(new CookAppsIapGeneral(), PurchaseProcessingResult.Pending, items, null);

            InitializeResult result = await CookAppsIap.Initialize(param);

            switch (result.Result)
            {
                case EnumResult.SUCCESS:
                    break;

                case EnumResult.FAIL:
                    switch (CookAppsIap.Instance.StoreType)
                    {
                        case CookAppsIap.EnumStoreType.GENERAL:
                            Debug.Log($"초기화 실패 : Code : {(InitializationFailureReason) result.FailReason.Code}, message : {result.FailReason.Message}");
                            break;
                    }

                    break;
            }
        }

        private List<ProductCatalogItem> LocalCreateProductItems()
        {
            var list = new List<ProductCatalogItem>();
            foreach (var shop in SpecDataManager.Inst.Shop.All)
            {
                if (shop.purchaseType != Enum_PurchaseType.IAP)
                    continue;
                list.Add(new ProductCatalogItem
                {
                    id = shop.product_id,
                    type = ProductType.Consumable,
                });
            }

            return list;
        }

        /// <summary>
        /// 가격 정보 가져오기
        /// </summary>
        public bool GetLocalizedPrice(string id, out string priceStr)
        {
            Product product = GetProduct(id);
            priceStr = product != null ? product.metadata.localizedPriceString : "로딩중";
            return product != null;
        }

        /// <summary>
        /// 상품 정보 가져오기
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private Product GetProduct(string id)
        {
            return CookAppsIap.Instance.GetProduct<Product>(id);
        }

        public async UniTask<List<ItemInfo>> PurchaseProduct(string productId)
        {
            PurchaseResult result = await CookAppsIap.Instance.Purchase(productId);
            MainUI.Inst.SetIndicator(true);
            switch (result.Result)
            {
                case EnumResult.SUCCESS:
                    Debug.Log($"구매 성공 - productId : {result.ProductId}, orderID : {result.OrderId}, isoCurrencyCode : {result.CurrencyCode}, localizedPrice : {result.LocalizedPrice},  purchaseData : " + result.Receipt);
                    _awaiter = new UniTaskCompletionSource<List<ItemInfo>>();
                    HandlingAfterPurchase(result);
                    var list = await _awaiter.Task;
                    MainUI.Inst.SetIndicator(false);
                    return list;

                case EnumResult.FAIL:
                    Debug.Log($"구매 실패 : code : {result.FailReason.Code}, message : {result.FailReason.Message}");
                    MainUI.Inst.SetIndicator(false);
                    break;
            }

            return null;
        }

        private async void HandlingAfterPurchase(PurchaseResult result)
        {
            //Pending 상태라면
            if (CookAppsIap.Instance.PurchaseProcessingResult == PurchaseProcessingResult.Pending)
            {
                if (Validate(result))
                {
                    CookAppsIap.Instance.ConfirmPendingPurchase(result.ProductId, async (isSuccess) =>
                    {
                        if (isSuccess)
                        {
                            CAppEventLite.ReportInAppPurchase(
                                result.OrderId,
                                result.ProductId,
                                result.CurrencyCode,
                                (double) result.LocalizedPrice,
                                result.Receipt);

                            var shop = SpecDataManager.Inst.Shop.Get(result.ProductId);
                            if (shop != null)
                            {
                                var list = shop.GetProductList();
                                UserManager.Inst.AddItem(list);
                                await UserManager.Inst.Save(true);
                                MainUI.Inst.SetIndicator(false);
                                _awaiter.TrySetResult(list);
                            }
                        }
                        else
                        {
                            Debug.LogError($"Consume 실패...");
                            MainUI.Inst.SetIndicator(false);
                        }
                    });
                }
                else
                {
                    MainUI.Inst.SetIndicator(false);
                }
            }
        }

        private bool Validate(PurchaseResult result)
        {
#if UNITY_EDITOR
            return true;
#endif
            try
            {
                var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);

                var receipt = validator.Validate(result.Receipt);
                // For informational purposes, we list the receipt(s)
                Debug.Log("Receipt is valid. Contents:");
                foreach (IPurchaseReceipt productReceipt in receipt)
                {
                    Debug.Log(productReceipt.productID);
                    Debug.Log(productReceipt.purchaseDate);
                    Debug.Log(productReceipt.transactionID);
                }

                return true;
            }
            catch (IAPSecurityException)
            {
                return false;
            }

            return false;
        }
    }
}
