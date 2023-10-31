import { NavLink } from "react-router-dom";
import { useContext } from "react";
import { UserContext } from "../../contexts/UserContext";
import "./styles/NavBarPartial.css";


const NavBarPartial = () =>{
    const {userUpdate, user} = useContext(UserContext);

    const logout = () =>{
        userUpdate({jwt: "", username: "", type: 0, rememberMe: false});
    }

    return(
        <nav>
            <NavLink to="/">Shotgun</NavLink>
            {
                user === null ? 
                <>
                </>
                :
                user.jwt === "" ? 
                <>
                    <NavLink to="/login">Login</NavLink>
                    <NavLink to="/register">Register</NavLink>
                </>
                :
                <>
                    <button onClick={() => logout()}>Logout</button>
                </>
            }
            
        </nav>
    );
}

export default NavBarPartial;