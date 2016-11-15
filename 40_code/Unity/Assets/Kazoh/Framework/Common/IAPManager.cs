using UnityEngine;
using UnityEngine.Purchasing;
using System;
using System.Collections;
using System.Collections.Generic;


// Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
public class IAPManager : MonoBehaviour, IStoreListener
{
    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    // Product identifiers for all products capable of being purchased: 
    // "convenience" general identifiers for use with Purchasing, and their store-specific identifier 
    // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers 
    // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

    // General product identifiers for the consumable, non-consumable, and subscription products.
    // Use these handles in the code to reference which product to purchase. Also use these values 
    // when defining the Product Identifiers on the store. Except, for illustration purposes, the 
    // kProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
    // specific mapping to Unity Purchasing's AddProduct, below.

    public static string[] kProductIDConsumable = {"eaf_item_01", "eaf_item_02", "eaf_item_03", "eaf_item_04", "eaf_item_05", "eaf_item_06"};
    public static string[] kProductIDNonConsumable = {"nonconsumable_item_01"};
    public static string[] kProductIDSubscription = {"subscription_item_01"};

    // Apple App Store-specific product identifier for the subscription product.
    private static string kProductNameAppleSubscription = "com.unity3d.subscription.new";

    // Google Play Store-specific product identifier subscription product.
    private static string kProductNameGooglePlaySubscription = "com.unity3d.subscription.original";

    private Action callback;

    public void Init()
    {
        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }
    }

    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Add a product to sell / restore by way of its identifier, associating the general identifier
        // with its store-specific identifiers.
        //builder.AddProduct(kProductIDConsumable, ProductType.Consumable);
        foreach (string _id in kProductIDConsumable)
        {
            builder.AddProduct(_id, ProductType.Consumable);
        }

        // Continue adding the non-consumable product.
        //foreach (string _id in kProductIDNonConsumable)
        //{
        //    builder.AddProduct(_id, ProductType.NonConsumable);
        //}

        // And finish adding the subscription product. Notice this uses store-specific IDs, illustrating
        // if the Product ID was configured differently between Apple and Google stores. Also note that
        // one uses the general kProductIDSubscription handle inside the game - the store-specific IDs 
        // must only be referenced here. 
        //foreach (string _id in kProductIDSubscription)
        //{
        //    builder.AddProduct(_id, ProductType.Subscription, new IDs(){
        //            { kProductNameAppleSubscription, AppleAppStore.Name },
        //            { kProductNameGooglePlaySubscription, GooglePlay.Name },
        //        });
        //}

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
    }

    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }


    public void BuyConsumable(int _idx, Action _callback = null)
    {
        // Buy the consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        try
        {
            if (_idx <= 0 || _idx > kProductIDConsumable.Length) throw new GameException(GameException.ErrorCode.InvalidParam);
            _idx--;
            callback = _callback;
            BuyProductID(kProductIDConsumable[_idx]);
        }
        catch (Exception e)
        {
            throw e;
        }
    }


    public void BuyNonConsumable(int _idx, Action _callback = null)
    {
        // Buy the non-consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        try
        {
            if (_idx <= 0 || _idx > kProductIDNonConsumable.Length) throw new GameException(GameException.ErrorCode.InvalidParam);
            callback = _callback;
            BuyProductID(kProductIDNonConsumable[_idx]);
        }
        catch (Exception e)
        {
            throw e;
        }
    }


    public void BuySubscription(int _idx, Action _callback = null)
    {
        // Buy the subscription product using its the general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        // Notice how we use the general product identifier in spite of this ID being mapped to
        // custom store-specific identifiers above.
        try
        {
            if (_idx <= 0 || _idx > kProductIDSubscription.Length) throw new GameException(GameException.ErrorCode.InvalidParam);
            callback = _callback;
            BuyProductID(kProductIDSubscription[_idx]);
        }
        catch (Exception e)
        {
            throw e;
        }
    }


    void BuyProductID(string productId)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
#if UNITY_EDITOR
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
#endif
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation  
                Debug.LogError("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                throw new GameException(GameException.ErrorCode.NotPurchasingProduct);
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            Debug.LogError("BuyProductID FAIL. Not initialized.");
            throw new GameException(GameException.ErrorCode.NotInitializedPurchaser);
        }
    }


    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.LogError("RestorePurchases FAIL. Not initialized.");
            throw new GameException(GameException.ErrorCode.FailRestorePurchases);
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
#if UNITY_EDITOR
            Debug.Log("RestorePurchases started ...");
#endif

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) => {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
#if UNITY_EDITOR
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
#endif
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.LogError("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            throw new GameException(GameException.ErrorCode.FailRestorePurchases);
        }
    }


    //  
    // --- IStoreListener
    //

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
#if UNITY_EDITOR
        Debug.Log("OnInitialized: PASS");
#endif

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.LogError("OnInitializeFailed InitializationFailureReason:" + error);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        // A consumable product has been purchased by this user.
        for(int i=0; i<kProductIDConsumable.Length; ++i)
        {
            if (string.Equals(args.purchasedProduct.definition.id, kProductIDConsumable[i], StringComparison.Ordinal))
            {
#if UNITY_EDITOR
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
#endif
                // The consumable item has been successfully purchased.
                if (callback != null) callback();
                return PurchaseProcessingResult.Complete;
            }
        }

        // Or ... a non-consumable product has been purchased by this user.
        for (int i = 0; i < kProductIDNonConsumable.Length; ++i)
        {
            if (string.Equals(args.purchasedProduct.definition.id, kProductIDNonConsumable[i], StringComparison.Ordinal))
            {
#if UNITY_EDITOR
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
#endif
                // The consumable item has been successfully purchased.
                if (callback != null) callback();
                return PurchaseProcessingResult.Complete;
            }
        }

        // Or ... a subscription product has been purchased by this user.
        for (int i = 0; i < kProductIDNonConsumable.Length; ++i)
        {
            if (string.Equals(args.purchasedProduct.definition.id, kProductIDSubscription[i], StringComparison.Ordinal))
            {
#if UNITY_EDITOR
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
#endif
                // The consumable item has been successfully purchased.
                if (callback != null) callback();
                return PurchaseProcessingResult.Complete;
            }
        }

        // Or ... an unknown product has been purchased by this user. Fill in additional products here....
        Debug.LogError(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        Debug.LogError(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }
}
