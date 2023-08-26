using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using UnityEngine;
using Logger = ZipsAR.Logger;

public class FirebaseAuthManager : Singleton<FirebaseAuthManager>
{
    private FirebaseAuth auth;
    // private FirebaseUser user;
    private bool isFBInit;

    private string testEmail = "testUser2@zipar.com";
    private string testPassword = "acbd1234";
    
    public void Init()
    {
        auth = FirebaseAuth.DefaultInstance;
        isFBInit = true;
        
        Logger.Log("FirebaseAuthManager Init");
    }
    
    private void CheckFirebaseInitialized()
    {
        if (!isFBInit)
        {
            Debug.LogError("Firebase is not initialized yet.");
        }
    }
    
    public void Create()
    {
        Logger.Log("FirebaseAuthManager Create");
        
        CheckFirebaseInitialized();
        
        auth.CreateUserWithEmailAndPasswordAsync(testEmail, testPassword).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }
            Logger.Log("Create Success");
            
            // Firebase user has been created.
            // Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                // user.DisplayName, user.UserId);
        });
    }

    public void Login()
    {
        Logger.Log("FirebaseAuthManager Login");

        CheckFirebaseInitialized();

        auth.SignInWithEmailAndPasswordAsync(testEmail, testPassword).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            Logger.Log("Login Success");
            // Debug.LogFormat("User signed in successfully: {0} ({1})",
                // user.DisplayName, user.UserId);
        });
    }

    public void Logout()
    {
        CheckFirebaseInitialized();

        auth.SignOut();
    }


}
