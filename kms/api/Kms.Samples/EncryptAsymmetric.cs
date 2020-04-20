/*
 * Copyright 2020 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// [START kms_encrypt_asymmetric]

using Google.Cloud.Kms.V1;
using System;
using System.Security.Cryptography;
using System.Text;

public class EncryptAsymmetricSample
{
    public byte[] EncryptAsymmetric(
      string projectId = "my-project", string locationId = "us-east1", string keyRingId = "my-key-ring", string keyId = "my-key", string keyVersionId = "123",
      string message = "Sample message")
    {
        // Create the client.
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();

        // Get the public key.
        PublicKey publicKey = client.GetPublicKey(new GetPublicKeyRequest
        {
            CryptoKeyVersionName = new CryptoKeyVersionName(projectId, locationId, keyRingId, keyId, keyVersionId),
        });

        // Split the key into blocks and base64-decode the PEM parts.
        string[] blocks = publicKey.Pem.Split("-", StringSplitOptions.RemoveEmptyEntries);
        byte[] pem = Convert.FromBase64String(blocks[1]);

        // Create a new RSA key.
        RSA rsa = RSA.Create();
        rsa.ImportSubjectPublicKeyInfo(pem, out _);

        // Convert the message into bytes. Cryptographic plaintexts and
        // ciphertexts are always byte arrays.
        byte[] plaintext = Encoding.UTF8.GetBytes(message);

        // Encrypt the data.
        byte[] ciphertext = rsa.Encrypt(plaintext, RSAEncryptionPadding.OaepSHA256);
        return ciphertext;
    }
}
// [END kms_encrypt_asymmetric]
