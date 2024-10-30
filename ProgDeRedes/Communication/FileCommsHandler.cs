using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Communication
{
    public class FileCommsHandler
    {
        public readonly ConversionHandler _conversionHandler;
        public readonly FileHandler _fileHandler;
        public readonly FileStreamHandler _fileStreamHandler;
        public readonly NetworkDataHelper _networkDataHelper;

        public FileCommsHandler(TcpClient tcpClient)
        {
            _conversionHandler = new ConversionHandler();
            _fileHandler = new FileHandler();
            _fileStreamHandler = new FileStreamHandler();
            _networkDataHelper = new NetworkDataHelper(tcpClient);
        }

        public async Task SendFile(string path)
        {
            if (await _fileHandler.FileExists(path))
            {
                var originalFileName = await _fileHandler.GetFileName(path);
                
                var fileName = Guid.NewGuid().ToString() + "." + originalFileName.Split('.').Last();                // ---> Enviar el largo del nombre del archivo
                await _networkDataHelper.SendAsync(_conversionHandler.ConvertIntToBytes(fileName.Length));
                // ---> Enviar el nombre del archivo
                await _networkDataHelper.SendAsync(_conversionHandler.ConvertStringToBytes(fileName));

                // ---> Obtener el tamaño del archivo
                long fileSize = await _fileHandler.GetFileSize(path);
                // ---> Enviar el tamaño del archivo
                var convertedFileSize = _conversionHandler.ConvertLongToBytes(fileSize);
                await _networkDataHelper.SendAsync(convertedFileSize);
                // ---> Enviar el archivo (pero con file stream)
                await SendFileWithStream(fileSize, path);
            }
            else
            {
                throw new Exception("File does not exist");
            }
        }

        public async Task<string> ReceiveFile()
        {
            // ---> Recibir el largo del nombre del archivo
            int fileNameSize = _conversionHandler.ConvertBytesToInt(
                await _networkDataHelper.ReceiveAsync(Protocol.FixedDataSize));
            // ---> Recibir el nombre del archivo
            string fileName = _conversionHandler.ConvertBytesToString(await _networkDataHelper.ReceiveAsync(fileNameSize));
            // ---> Recibir el largo del archivo
            long fileSize = _conversionHandler.ConvertBytesToLong(
                await _networkDataHelper.ReceiveAsync(Protocol.FixedFileSize));
            // ---> Recibir el archivo
            await ReceiveFileWithStreams(fileSize, fileName);

            return fileName;
        }

        private async Task SendFileWithStream(long fileSize, string path)
        {
            long fileParts = await Protocol.CalculateFileParts(fileSize);
            long offset = 0;
            long currentPart = 1;  
         
            //Mientras tengo un segmento a enviar
            while (fileSize > offset)
            {
                byte[] data;
                //Es el último segmento?
                if (currentPart == fileParts)
                {
                    var lastPartSize = (int)(fileSize - offset);
                    //1- Leo de disco el último segmento
                    //2- Guardo el último segmento en un buffer
                    data = await _fileStreamHandler.Read(path, offset, lastPartSize); //Puntos 1 y 2
                    offset += lastPartSize;
                }
                else
                {
                    //1- Leo de disco el segmento
                    //2- Guardo ese segmento en un buffer
                    data = await _fileStreamHandler.Read(path, offset, Protocol.MaxPacketSize);
                    offset += Protocol.MaxPacketSize;
                }

                await _networkDataHelper.SendAsync(data); //3- Envío ese segmento a travez de la red
                currentPart++;
            }
        }

        private async Task ReceiveFileWithStreams(long fileSize, string fileName)
        {
            long fileParts = await Protocol.CalculateFileParts(fileSize);
            long offset = 0;
            long currentPart = 1;

            //Mientras tengo partes para recibir
            while (fileSize > offset)
            {
                byte[] data;
                //1- Me fijo si es la ultima parte
                if (currentPart == fileParts)
                {
                    //1.1 - Si es, recibo la ultima parte
                    var lastPartSize = (int)(fileSize - offset);
                    data = await _networkDataHelper.ReceiveAsync(lastPartSize);
                    offset += lastPartSize;
                }
                else
                {
                    //2.2- Si no, recibo una parte cualquiera
                    data = await _networkDataHelper.ReceiveAsync(Protocol.MaxPacketSize);
                    offset += Protocol.MaxPacketSize;
                }
                //3- Escribo esa parte del archivo a disco
                await _fileStreamHandler.Write(fileName, data);
                currentPart++;
            }
        }
    }
}

