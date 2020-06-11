using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class IcCSVFile 
{
    public static int NEXTLINE = 1;
    public static int NEXTWORD = 2;

    public static char cSeparator = ',';
    public static char cLF = '\n';//0x0A
    public static char cQuotationMark = '"';
    public static char cCR = '\r';//0x0D

    private byte[] m_buf;
    private int m_offset = 0;

    private char[] m_readBuf = new char[1024];//ensure enough space
    private int m_readOffset = 0;
    private int m_readLength = 0;
    
    //log
    private string m_fileName;

    public IcCSVFile(string szInfo)
    {
        m_buf = System.Text.Encoding.UTF8.GetBytes(szInfo);

        #region 用于把utf-8 BOM格式转换为utf_8无BOM格式
        int length = m_buf.Length;
        if (length >= 2)
		{
            if ((m_buf[0] == '\xFF' && m_buf[1] == '\xFE') 
                || (m_buf[0] == '\xFE' && m_buf[1] == '\xFF'))
            {
                byte[] tmp = new byte[length - 2];
                Array.Copy(m_buf, 2, tmp, 0, length - 2);
                m_buf = tmp;
            }
            else if(length >= 3)
            {
                if (m_buf[0] == '\xEF' && m_buf[1] == '\xBB' && m_buf[2] == '\xBF')
                {
                    byte[] tmp = new byte[length - 3];
                    Array.Copy(m_buf, 3, tmp, 0, length - 3);
                    m_buf = tmp;
                }
            }
        }
        #endregion

#if ICDEBUG
        Debug.LogDevelop(string.Format("Init CSVAsset... "));
#endif
    }

	public IcCSVFile(TextAsset asset, string fileName)
	{
        m_fileName = fileName;
        if (string.IsNullOrEmpty(m_fileName))
        {
            m_fileName = "fileName_not_provide";
        }

        m_buf = asset.bytes;
#if ICDEBUG
        Debug.LogDevelop(string.Format("Init CSVFile... {0}", m_fileName));
#endif
	}

    public IcCSVFile(byte[] bytes, string fileName)
    {
        m_fileName = fileName;
        if (string.IsNullOrEmpty(m_fileName))
        {
            m_fileName = "fileName_not_provide";
        }

        m_buf = bytes;
#if ICDEBUG
        Debug.LogDevelop(string.Format("Init CSVFile... {0}", m_fileName));
#endif
    }

    private void MoveNext(int iOperate)
    {
        if (iOperate == NEXTLINE)
        {
            NextLine();
        }
        else if (iOperate == NEXTWORD)
        {
            //do nothing
        }
    }

    public bool SkipToKeyWord(string keyword, int iOperate)
    {
        if(string.IsNullOrEmpty(keyword))
            return false;

        bool bFound = false;
        int bufEnd = m_buf.Length;
        while (m_offset < bufEnd)
        {
            string str;
            if (!TextGetString(out str, NEXTWORD))
                return false;

            if (str == keyword)
            {
                bFound = true;
                break;
            }
        }

        if (bFound)
        {
            MoveNext(iOperate);
            return true;
        }

        Debug.LogWarning(string.Format("keyword={0} not found.", keyword));
        return false;
    }

    public bool NextLine()
    {
        int bufEnd = m_buf.Length;

        while (m_offset < bufEnd)
        {
            if (m_buf[m_offset] == cLF)
            {
                m_offset++;
                break;
            }
            m_offset++;
        }
        return m_offset < bufEnd;
    }

    private bool TextGetWord(int iOperate)
    {
        int bufEnd = m_buf.Length;

        if (m_offset >= bufEnd)
            return false;

        m_readOffset = -1;
        m_readLength = 0;
        int countQM = 0;    //QuotationMark count //is in "" ?

        if (m_buf[m_offset] == cQuotationMark)
        {
            countQM = 1;
            m_offset++;
        }
		
        while (m_offset < bufEnd)
        {
            if (m_buf[m_offset] == cLF)
            {
                break;
            }
            else if (m_buf[m_offset] == cQuotationMark)
            {
                if ((m_offset + 1) < bufEnd)
                {
                    if (m_buf[m_offset + 1] == cQuotationMark)
                    {
                        m_offset++;
                    }
                    else
                    {
                        if (countQM == 1 && (m_buf[m_offset + 1] == cSeparator || m_buf[m_offset + 1] == cCR || m_buf[m_offset + 1] == cLF))
                        {
                            m_offset++;
                            countQM = 0;
                            break;
                        }
                        else
                        {   //file format error
                            return false;
                        }
                    }
                }
                else
                {
                    if (countQM == 1)
                    {
                        m_offset++;
                        countQM = 0;
                        break;
                    }
                    else
                    {   //file format error
                        return false;
                    }
                }
            }
            else if (m_buf[m_offset] == cSeparator)
            {
                if (countQM == 0)
                {
                    break;
                }
            }
            if (m_buf[m_offset] != cCR)
            {
                if (m_readOffset == -1)
                {
                    m_readOffset = m_offset;
                }
                m_readLength++;
            }
            m_offset++;
        }//end of while

        while ((m_offset < bufEnd) && (m_buf[m_offset] == cCR || m_buf[m_offset] == cSeparator))
        {
            m_offset++;
        }
        MoveNext(iOperate);
        return m_readLength != 0;
    }


    public bool TextGetInt(out int num, int iOperate)
    {
        return TextGetInt(out num, iOperate, "");
    }

    public bool TextGetInt(out int num, int iOperate, string logMsg) 
	{
        num = 0;
		string str = string.Empty;
		
        try
        {
            if (!TextGetString(out str,iOperate) || string.IsNullOrEmpty(str))
			{
                Debug.LogWarning(string.Format("parse file({0}) TextGetInt warning, string={1}, logMsg={2}", m_fileName, str, logMsg));
				return false;
			}
			
			try
			{
				num = int.Parse(str);
			}
			catch(FormatException ex)
			{
                ex.GetType();
				float f = float.Parse(str);
				num = (int)f;
			}
			catch(OverflowException ex)//9999999999
			{
                Debug.LogWarning(string.Format("parse file({0}) warning, exception={1}, originalString={2}, logMsg={3}", m_fileName, ex, str, logMsg));
				num = int.MaxValue;
			}
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError(string.Format("parse file({0}) failed, exception={1}, originalString={2}, logMsg={3}", m_fileName, ex, str, logMsg));
            return false;
        }

        return true;
	}

    public bool TextGetFloat(out float num, int iOperate)
    {
        return TextGetFloat(out num, iOperate, "");
    }

    public bool TextGetFloat(out float num, int iOperate, string logMsg) 
	{
        num = 0f;
		string str = string.Empty;
		
        try
        {
            if (!TextGetString(out str,iOperate) || string.IsNullOrEmpty(str))
			{
                Debug.LogWarning(string.Format("parse file({0}) TextGetFloat warning, string={1}, logMsg={2}", m_fileName, str, logMsg));
				return false;
			}
			
			num = float.Parse(str);
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError(string.Format("parse file({0}) failed, exception={1}, originalString={2}, logMsg={3}", m_fileName, ex, str, logMsg));
            return false;
        }

        return true;
	}

    public bool TextGetString(out string str, int iOperate)
    {
        return TextGetString(out str, iOperate, "");
    }

    public bool TextGetString(out string str, int iOperate, string logMsg)
	{
        str = "";

        try
        {
            if (!TextGetWord(iOperate))
                return false;

            //char[] destChars = new char[Encoding.UTF8.GetCharCount(m_buf, m_readOffset, m_readLength)];
            //Encoding.UTF8.GetChars(m_buf, m_readOffset, m_readLength, destChars, 0);
            //str = new string(destChars);

            int charCount = Encoding.UTF8.GetCharCount(m_buf, m_readOffset, m_readLength);
            Encoding.UTF8.GetChars(m_buf, m_readOffset, m_readLength, m_readBuf, 0);
            str = new string(m_readBuf, 0, charCount); 
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError(string.Format("parse file({0}) failed, exception={1}, logMsg={2}", m_fileName, ex, logMsg));
            return false;
        }

        return true;
	}
}
