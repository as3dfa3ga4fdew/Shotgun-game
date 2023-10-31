import { useRef, useState, useContext } from "react";
import { IsValidUsername, IsValidPassword } from "../../helpers/ValidateInput";
import { RsaEncrypt } from "../../helpers/RsaHandler";
import { UserContext } from "../../contexts/UserContext";
import jwt_decode from "jwt-decode";
import { useNavigate } from "react-router-dom";

const LoginPage = () =>{
    const navigation = useNavigate();
    const {userUpdate, user} = useContext(UserContext);
    const [errorMessage, setErrorMessage] = useState(null);

    let usernameRef = useRef();
    let passwordRef = useRef();

    const [rememberMe, setRememberMe] = useState(false);
    const rememberMeHandler = () => {
        setRememberMe(!rememberMe);
    }

    const login = async () =>{
        const username = usernameRef.current.value;
        const password = passwordRef.current.value;
        
        //Validates if username is valid
        if(!IsValidUsername(username))
        {
            setErrorMessage("Invalid username.");
            return;
        }
        //Validates if password is valid
        if(!IsValidPassword(password))
        {
            setErrorMessage("Invalid password.");
            return;
        }

        try{
            let response;
            let result;

            //Get rsa public key from server
            response = await fetch("https://localhost:5000/api/auth/publickey");
            if(response.ok !== true)
            {
                setErrorMessage("Ooops something went wrong please try again later... 1");
                return;
            }

            result = await response.json();
            
            let publicKey = result.rsaPublicKey;

            //Encrypts username and password and created a object
            let body = {
                username: RsaEncrypt(username, publicKey),
                password: RsaEncrypt(password, publicKey)
            };

            console.log(JSON.stringify(body));
            
            //Sends the login information to api
            response = await fetch("https://localhost:5000/api/auth/login", {
                method: "post",
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(body)
            });

            if(response.ok !== true)
            {
                setErrorMessage("Invalid username or password.");
                return;
            }

            result = await response.json();

            setErrorMessage("success");

            //Decodes jwt token 
            const token = result.jwt;
            const decoded = jwt_decode(token);

            //Creates a user object from jwt token
            let userBuilder = {
                jwt: result.jwt,
                username: decoded.username,
                type: decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"],
                rememberMe: rememberMe
            };
            
            userUpdate(userBuilder);

            //redirect to home page
            navigation("/");
        }
        catch(error){
            console.error(error);
            setErrorMessage("Ooops something went wrong please try again later...");
            return;
        }
    }

    return(
        <div className="login-page">
            <div>
                <div className="input-container">
                    <p>Username</p>
                    <input type="text" ref={usernameRef}/>
                </div>
                <div className="input-container">
                    <p>Password</p>
                    <input type="password" ref={passwordRef}/>
                </div>
                <div>
                    <p>Remembr me</p>
                    <input type="checkbox" onChange={rememberMeHandler} checked={rememberMe}/>
                </div>
                <div className="error-container">
                    <p>{errorMessage == null ? "" : errorMessage}</p>
                </div>
                <div className="button-container">
                    <button onClick={() => login()}>Login</button>
                </div>
            </div>
        </div>
    );
}

export default LoginPage;