/* global JSEncrypt */

//Encrypts a string using a provided public key
const RsaEncrypt = (message, publicKey) => {
    try
    {
        const jsEncrypt = new JSEncrypt();

        jsEncrypt.setPublicKey(publicKey);
    
        return jsEncrypt.encrypt(message); 
    }
    catch(error)
    {
        console.error(error);
    }
}

export {RsaEncrypt};