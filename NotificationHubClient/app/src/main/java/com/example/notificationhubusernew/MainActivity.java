package com.example.notificationhubusernew;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;

import android.os.AsyncTask;
import android.os.Bundle;
import android.util.Log;
import android.widget.Toast;

import com.google.android.gms.tasks.OnCompleteListener;
import com.google.android.gms.tasks.Task;
import com.google.firebase.messaging.FirebaseMessaging;

import org.json.JSONException;

import java.io.IOException;
import java.util.HashSet;

public class MainActivity extends AppCompatActivity {
    private static final String TAG = "MyActivity";

    private RegisterClient registerClient;
    private static final String BACKEND_ENDPOINT = "http://demofelcunotificationhub-win.westeurope.cloudapp.azure.com:5000";
    String FCM_token = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        registerClient = new RegisterClient(this, BACKEND_ENDPOINT);

        FirebaseMessaging.getInstance().getToken()
                .addOnCompleteListener(new OnCompleteListener<String>() {
                    @Override
                    public void onComplete(@NonNull Task<String> task) {
                        if (!task.isSuccessful()) {
                            Log.w("", "Fetching FCM registration token failed", task.getException());
                            return;
                        }

                        FCM_token = task.getResult();
                        String msg="The received token is"+ FCM_token;
                        Log.d(TAG, msg);
                        Toast.makeText(MainActivity.this, msg, Toast.LENGTH_SHORT).show();
                        RegisterClient(FCM_token);
                    }

                });
    }

    public void RegisterClient(String token) {
        new AsyncTask<Object, Object, Object>() {
            @Override
            protected Object doInBackground(Object... params) {
                try {
                    HashSet<String>  tags= new HashSet<String>();
                    tags.add("username:gianluigi");
                   registerClient.register(FCM_token,tags);
                } catch (Exception e) {
                    return e;
                }
                return null;
            }

        }.execute(null, null, null);
    }
}