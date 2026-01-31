// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const getUsernameValue = () => {
    const text = document.getElementById("userNameSpan").innerText;
    return text.length === 0 ? null : text;
};