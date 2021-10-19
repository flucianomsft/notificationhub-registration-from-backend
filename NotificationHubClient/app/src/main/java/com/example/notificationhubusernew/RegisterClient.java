package com.example.notificationhubusernew;

import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.util.Set;
import java.util.UUID;

import org.apache.http.HttpResponse;
import org.apache.http.HttpStatus;
import org.apache.http.client.ClientProtocolException;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.client.methods.HttpPut;
import org.apache.http.client.methods.HttpUriRequest;
import org.apache.http.entity.StringEntity;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.util.EntityUtils;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import android.content.Context;
import android.content.SharedPreferences;
import android.util.Log;

public class RegisterClient {
    private static final String PREFS_NAME = "ANHSettings";
    private static final String REGID_SETTING_NAME = "ANHRegistrationId";
    private String Backend_Endpoint;
    SharedPreferences settings;
    protected HttpClient httpClient;
    private String authorizationHeader;

    public RegisterClient(Context context, String backendEndpoint) {
        super();
        this.settings = context.getSharedPreferences(PREFS_NAME, 0);
        httpClient =  new DefaultHttpClient();
        Backend_Endpoint = backendEndpoint + "/register";
    }

    public String getAuthorizationHeader() {
        return authorizationHeader;
    }

    public void setAuthorizationHeader(String authorizationHeader) {
        this.authorizationHeader = authorizationHeader;
    }

    public void register(String handle, Set<String> tags) throws ClientProtocolException, IOException, JSONException {
        String registrationId = retrieveInstallationIdOrRequestNewOne();

        JSONObject deviceInfo = new JSONObject();
        deviceInfo.put("Platform", "fcm");
        deviceInfo.put("Handle", handle);
        deviceInfo.put("Tags", new JSONArray(tags));

        int statusCode = upsertRegistration(registrationId, deviceInfo);

        if (statusCode == HttpStatus.SC_OK) {
            return;
        } else {
            Log.e("RegisterClient", "Error upserting registration: " + statusCode);
            throw new RuntimeException("Error upserting registration");
        }
    }

    private int upsertRegistration(String registrationId, JSONObject deviceInfo)
            throws UnsupportedEncodingException, IOException,
            ClientProtocolException {
        HttpPut request = new HttpPut(Backend_Endpoint+"/"+registrationId);
        request.setEntity(new StringEntity(deviceInfo.toString()));
        request.addHeader("Authorization", "Basic "+authorizationHeader);
        request.addHeader("Content-Type", "application/json");
        HttpResponse response = httpClient.execute(request);
        int statusCode = response.getStatusLine().getStatusCode();
        return statusCode;
    }

    private String retrieveInstallationIdOrRequestNewOne() throws ClientProtocolException, IOException {
        if (settings.contains(REGID_SETTING_NAME))
            return settings.getString(REGID_SETTING_NAME, null);
        String  uniqueID = UUID.randomUUID().toString();
        settings.edit().putString(REGID_SETTING_NAME, uniqueID).commit();
        return uniqueID;
    }
}