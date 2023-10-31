import { Route, Routes } from "react-router-dom";
import HomePage from "./home/HomePage";
import LoginPage from "./login/LoginPage";
import RegisterPage from "./register/RegisterPage";

//Routes paths
const Routing = () =>{
    return(
        <Routes>
            <Route path='/' element={<HomePage/>}/>
            <Route path='/login' element={<LoginPage/>}/>
            <Route path="/register" element={<RegisterPage/>}/>
        </Routes>
    );
}

export default Routing;